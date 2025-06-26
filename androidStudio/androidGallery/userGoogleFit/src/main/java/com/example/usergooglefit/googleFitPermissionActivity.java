package com.example.usergooglefit;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.net.Uri;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.fitness.*;

public class googleFitPermissionActivity extends Activity {

    private static final int ACTIVITY_RECOGNITION_PERMISSION_CODE = 1001;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACTIVITY_RECOGNITION)
                    != PackageManager.PERMISSION_GRANTED) {

                ActivityCompat.requestPermissions(this,
                        new String[]{android.Manifest.permission.ACTIVITY_RECOGNITION},
                        ACTIVITY_RECOGNITION_PERMISSION_CODE);

                return; // 권한 요청 후 종료, 결과는 onRequestPermissionsResult에서 처리
            }
        }

        requestFitPermissions();
    }

    private void requestFitPermissions() {
        GoogleSignInAccount account = GoogleSignIn.getAccountForExtension(this, googleFit.FIT_OPTIONS);

        if (!GoogleSignIn.hasPermissions(account, googleFit.FIT_OPTIONS)) {
            GoogleSignIn.requestPermissions(
                    this,
                    googleFit.GOOGLE_FIT_PERMISSIONS_REQUEST_CODE,
                    account,
                    googleFit.FIT_OPTIONS
            );
        } else {
            startSensorAndExit();
        }
    }

    // Android 권한 요청 결과 처리
    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        if (requestCode == ACTIVITY_RECOGNITION_PERMISSION_CODE) {
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                requestFitPermissions();
            } else {
                Log.w(googleFit.TAG, "신체 활동 권한 거부됨.");
                finish();
            }
        } else {
            super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    // Google Fit OAuth 처리 결과
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == googleFit.GOOGLE_FIT_PERMISSIONS_REQUEST_CODE && resultCode == RESULT_OK) {
            startSensorAndExit();
        } else {
            Log.w(googleFit.TAG, "Google Fit 권한이 거부되었습니다.");
            finish();
        }
    }

    private void startSensorAndExit() {
        googleFit.subscribeSensor(this);
        finish();
    }
}
