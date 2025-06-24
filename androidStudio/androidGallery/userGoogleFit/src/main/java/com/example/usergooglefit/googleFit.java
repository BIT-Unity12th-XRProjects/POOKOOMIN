package com.example.usergooglefit;

import android.app.Activity;
import android.util.Log;
import android.widget.Toast;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.fitness.*;
import com.google.android.gms.fitness.data.*;
import com.google.android.gms.fitness.request.DataReadRequest;
import com.google.android.gms.tasks.*;

import java.util.List;
import java.util.concurrent.TimeUnit;

public class googleFit {
    private static final String TAG = "GoogleFitPlugin";
    private static final int GOOGLE_FIT_PERMISSIONS_REQUEST_CODE = 9001;

    public static void requestPermission(Activity activity) {
        FitnessOptions fitnessOptions = FitnessOptions.builder()
                .addDataType(DataType.TYPE_STEP_COUNT_DELTA, FitnessOptions.ACCESS_READ)
                .addDataType(DataType.AGGREGATE_STEP_COUNT_DELTA, FitnessOptions.ACCESS_READ)
                .build();

        GoogleSignInAccount account = GoogleSignIn.getAccountForExtension(activity, fitnessOptions);

        if (!GoogleSignIn.hasPermissions(account, fitnessOptions)) {
            GoogleSignIn.requestPermissions(
                    activity,
                    GOOGLE_FIT_PERMISSIONS_REQUEST_CODE,
                    account,
                    fitnessOptions
            );
        } else {
            Log.i(TAG, "Already has permission.");
        }
    }

    public static void getTodayStepCount(Activity activity) {
        GoogleSignInAccount account = GoogleSignIn.getAccountForExtension(activity,
                FitnessOptions.builder()
                        .addDataType(DataType.TYPE_STEP_COUNT_DELTA, FitnessOptions.ACCESS_READ)
                        .addDataType(DataType.AGGREGATE_STEP_COUNT_DELTA, FitnessOptions.ACCESS_READ)
                        .build());

        long end = System.currentTimeMillis();
        long start = end - TimeUnit.DAYS.toMillis(1);

        DataReadRequest readRequest = new DataReadRequest.Builder()
                .aggregate(DataType.TYPE_STEP_COUNT_DELTA, DataType.AGGREGATE_STEP_COUNT_DELTA)
                .bucketByTime(1, TimeUnit.DAYS)
                .setTimeRange(start, end, TimeUnit.MILLISECONDS)
                .build();

        Fitness.getHistoryClient(activity, account)
                .readData(readRequest)
                .addOnSuccessListener(response -> {
                    int totalSteps = 0;
                    for (Bucket bucket : response.getBuckets()) {
                        for (DataSet dataSet : bucket.getDataSets()) {
                            for (DataPoint dp : dataSet.getDataPoints()) {
                                for (Field field : dp.getDataType().getFields()) {
                                    totalSteps += dp.getValue(field).asInt();
                                }
                            }
                        }
                    }
                })
                .addOnFailureListener(e -> {
                    Log.e(TAG, "Failed to read steps", e);
                    Toast.makeText(activity, "Error reading steps", Toast.LENGTH_SHORT).show();
                });
    }
}
