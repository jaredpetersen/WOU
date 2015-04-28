package com.jaredpetersen.hiccup;

import android.app.Fragment;

import java.util.Collection;

/**
 * Created by jaredpetersen on 4/27/15.
 */
public class Globals{
    private static Globals instance;

    // Global variable
    private String userID;
    private CollectionFragment collectionFragment;

    // Restrict the constructor from being instantiated
    private Globals(){}

    public void setData(String id){
        this.userID=id;
    }
    public String getData(){
        return this.userID;
    }
    public void setCollection(CollectionFragment fragment)
    {
        collectionFragment = fragment;
    }
    public CollectionFragment getCollection()
    {
        return collectionFragment;
    }

    public static synchronized Globals getInstance(){
        if(instance==null){
            instance=new Globals();
        }
        return instance;
    }
}