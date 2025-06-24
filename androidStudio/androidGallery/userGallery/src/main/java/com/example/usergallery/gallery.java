package com.example.usergallery;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.provider.MediaStore;
import android.util.Base64;

import java.io.ByteArrayOutputStream;

public class gallery {

    // 1) 가장 최근 이미지 한 장 꺼내서 Unity로 전달
    public static String getFirstImage(Activity activity) {
        Cursor cursor = activity.getContentResolver().query(
                MediaStore.Images.Media.EXTERNAL_CONTENT_URI,
                null,
                null,
                null,
                MediaStore.Images.Media.DATE_ADDED + " DESC" // 최근 추가된 순
        );

        if (cursor != null && cursor.moveToFirst()) {
            // 이미지 경로 찾기
            int columnIndex = cursor.getColumnIndex(MediaStore.Images.Media.DATA);
            String imagePath = cursor.getString(columnIndex);
            cursor.close();

            // 이미지 -> Bitmap -> Base64 변환
            Bitmap bitmap = BitmapFactory.decodeFile(imagePath);
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.JPEG, 80, stream);
            byte[] imageBytes = stream.toByteArray();
            return Base64.encodeToString(imageBytes, Base64.DEFAULT);
        }

        return ""; // 실패 시 빈 문자열 반환
    }

    // 2) 갤러리 앱 띄우기
    public static void openGallery(Activity activity) {
        if (activity == null) return;

        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setType("image/*");
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        activity.startActivity(intent);
    }
}
