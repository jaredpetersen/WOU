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
    private boolean collectionLoaded = false;
    private MetricsFragment metricsFragment;
    private boolean metricsLoaded = false;

    // Restrict the constructor from being instantiated
    private Globals(){}

    public void setData(String id)
    {
        this.userID=id;
    }

    public String getData()
    {
        return this.userID;
    }

    // Set the fragments
    public void setCollection(CollectionFragment fragment)
    {
        collectionFragment = fragment;
    }
    public void setMetrics(MetricsFragment fragment)
    {
        metricsFragment = fragment;
    }

    // Get the fragment data
    public CollectionFragment getCollection()
    {
        return collectionFragment;
    }
    public MetricsFragment getMetrics() { return metricsFragment; }

    public boolean getCollectionLoadedStatus()
    {
        return collectionLoaded;
    }
    public boolean getMetricsLoadedStatus()
    {
        return metricsLoaded;
    }

    public void setCollectionLoadedStatus(boolean status)
    {
        collectionLoaded = status;
    }

    public static synchronized Globals getInstance(){
        if(instance==null){
            instance=new Globals();
        }
        return instance;
    }
}