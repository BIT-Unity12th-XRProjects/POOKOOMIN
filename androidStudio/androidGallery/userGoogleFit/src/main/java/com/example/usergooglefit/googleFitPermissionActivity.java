package com.example.usergooglefit;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.net.Uri;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.fitness.*;

public class googleFitPermissionActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        GoogleSignInAccount account = GoogleSignIn.getAccountForExtension(this, googleFit.FIT_OPTIONS);

        //if (!isFitAppInstalled()) {
        //    redirectToPlayStore(); // Play Store로 이동 후 finish
        //    return;
        //}

        if (!GoogleSignIn.hasPermissions(account, googleFit.FIT_OPTIONS)) {
            GoogleSignIn.requestPermissions(
                    this,
                    googleFit.GOOGLE_FIT_PERMISSIONS_REQUEST_CODE,
                    account,
                    googleFit.FIT_OPTIONS);
        } else {
            startSensorAndExit();
        }
    }

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

    //구글 핏 없으면 다운로드하게 유도
    private void redirectToPlayStore() {
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setData(Uri.parse("market://details?id=com.google.android.apps.fitness"));
        startActivity(intent);
        finish(); // 여기서 종료 → 유저가 직접 다시 앱 열도록 유도
    }

    private boolean isFitAppInstalled() {
        try {
            getPackageManager().getPackageInfo("com.google.android.apps.fitness", 0);
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}
