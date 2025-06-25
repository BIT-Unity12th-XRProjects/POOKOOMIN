package com.example.usergooglefit;

import java.util.concurrent.TimeUnit;
import com.google.android.gms.fitness.data.DataType;
import com.google.android.gms.fitness.request.DataReadRequest;

import com.google.android.gms.fitness.Fitness;
import com.google.android.gms.fitness.HistoryClient;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import java.util.Calendar;
import java.util.Date;

import android.app.Activity;
import android.util.Log;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.fitness.*;
import com.google.android.gms.fitness.data.*;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.fitness.request.SensorRequest;
import com.google.android.gms.fitness.result.DataReadResponse;


public class googleFit {
    public static final String TAG = "GoogleFitPlugin";
    public static final int GOOGLE_FIT_PERMISSIONS_REQUEST_CODE = 9001;
    public static final FitnessOptions FIT_OPTIONS = FitnessOptions.builder()
            .addDataType(DataType.TYPE_STEP_COUNT_CUMULATIVE , FitnessOptions.ACCESS_READ)
            .build();

    //@TK : libs에 unity 두면 중복 되서 빌드 x, 따로 한번 캐싱해야함.
    private static Class<?> unityPlayer;
    private static java.lang.reflect.Field currentActivityField;
    private static java.lang.reflect.Method unitySendMessageMethod;

    // 1) 권한이 승인된 직후나, 이미 승인 상태면 여기서 센서 구독을 시작

    // 클래스 초기화 메서드 (한번만 호출)
    private static void initUnityReflection() throws Exception {
        if (unityPlayer == null) {
            unityPlayer = Class.forName("com.unity3d.player.UnityPlayer");
            currentActivityField = unityPlayer.getField("currentActivity");
            unitySendMessageMethod = unityPlayer.getMethod("UnitySendMessage", String.class, String.class, String.class);
        }
    }

    public static void getStepData(Activity activity) {

        try {
            initUnityReflection();
        } catch (Exception e) {
            Log.e(TAG, "Unity reflection 초기화 실패", e);
            return;
        }

        final Calendar cal = Calendar.getInstance();
        Date now = Calendar.getInstance().getTime();
        cal.setTime(now);

        // 시작 시간
        cal.set(cal.get(Calendar.YEAR), cal.get(Calendar.MONTH),
                cal.get(Calendar.DAY_OF_MONTH), 0, 0, 0);
        long startTime = cal.getTimeInMillis();

        // 종료 시간
        cal.set(cal.get(Calendar.YEAR), cal.get(Calendar.MONTH),
                cal.get(Calendar.DAY_OF_MONTH), 23, 59, 59);
        long endTime = cal.getTimeInMillis();

        Fitness.getHistoryClient(activity,
                        GoogleSignIn.getLastSignedInAccount(activity))
                .readData(new DataReadRequest.Builder()
                        .read(DataType.TYPE_STEP_COUNT_DELTA) // Raw 걸음 수
                        .setTimeRange(startTime, endTime, TimeUnit.MILLISECONDS)
                        .build())
                .addOnSuccessListener(new OnSuccessListener<DataReadResponse>() {
                    @Override
                    public void onSuccess(DataReadResponse response) {
                        DataSet dataSet = response.getDataSet(DataType.TYPE_STEP_COUNT_DELTA);
                        Log.i(TAG, "Data returned for Data type: " + dataSet.getDataType().getName());
                        int stepCount = 0;

                        for (DataPoint dp : dataSet.getDataPoints()) {
                            Log.i(TAG, "Data point:");
                            Log.i(TAG, "\tType: " + dp.getDataType().getName());
                            for (Field field : dp.getDataType().getFields()) {
                                stepCount += dp.getValue(field).asInt();
                            }
                        }

                        try {
                            unitySendMessageMethod.invoke(null, "GoogleFitService", "onStepCountChanged", String.valueOf(stepCount));
                        } catch (Exception e) {
                            Log.e(TAG, "UnitySendMessage invoke 실패", e);
                        }
                    }
                });
    }
}
