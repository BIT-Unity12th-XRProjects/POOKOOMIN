package com.example.usergooglefit;

import android.app.Activity;
import android.util.Log;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.fitness.*;
import com.google.android.gms.fitness.data.*;
import com.google.android.gms.fitness.request.SensorRequest;

import java.util.concurrent.TimeUnit;

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
    public static void subscribeSensor(Activity activity) {

        try {
            initUnityReflection();
        } catch (Exception e) {
            Log.e(TAG, "Unity reflection 초기화 실패", e);
            return;
        }

        SensorsClient sensorsClient = Fitness.getSensorsClient(
                activity,
                GoogleSignIn.getAccountForExtension(activity, FIT_OPTIONS));

        SensorRequest request = new SensorRequest.Builder()
                .setDataType(DataType.TYPE_STEP_COUNT_CUMULATIVE)
                .setSamplingRate(5, TimeUnit.SECONDS)
                .build();

        sensorsClient.add(
                request,
                dataPoint -> {
                    for (Field field : dataPoint.getDataType().getFields()) {
                        int stepCount = dataPoint.getValue(field).asInt();
                        // Unity로 전달
                        try {
                            Object currentActivity = currentActivityField.get(null);
                            unitySendMessageMethod.invoke(null, "GoogleFitService", "onStepCountChanged", String.valueOf(stepCount));
                        } catch (Exception e) {
                            Log.e(TAG, "Unity로 메시지 전달 실패", e);
                        }
                    }
                }
        ).addOnFailureListener(e -> Log.e(TAG, "센서 구독 실패", e));
    }

    // 클래스 초기화 메서드 (한번만 호출)
    private static void initUnityReflection() throws Exception {
        if (unityPlayer == null) {
            unityPlayer = Class.forName("com.unity3d.player.UnityPlayer");
            currentActivityField = unityPlayer.getField("currentActivity");
            unitySendMessageMethod = unityPlayer.getMethod("UnitySendMessage", String.class, String.class, String.class);
        }
    }
}
