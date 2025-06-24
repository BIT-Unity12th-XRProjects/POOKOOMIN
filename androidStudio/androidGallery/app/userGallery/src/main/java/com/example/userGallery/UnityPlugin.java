package com.example.userGallery;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;

public class UnityPlugin {
    private static UnityPlugin m_Instance;

    public Context context;

    public static UnityPlugin Instance(){
        if(m_Instance == null){
            m_Instance = new UnityPlugin();
        }

        return m_Instance;
    }

    private void setContext(Context context){
        this.context = context;
    }

    public static void openGallery(Activity activity){
    }
}
