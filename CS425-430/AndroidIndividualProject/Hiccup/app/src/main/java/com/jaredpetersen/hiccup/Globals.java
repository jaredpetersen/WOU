package com.jaredpetersen.hiccup;

/**
 * Created by jaredpetersen on 4/27/15.
 */
public class Globals{
    private static Globals instance;

    // Global variable
    private String userID;

    // Restrict the constructor from being instantiated
    private Globals(){}

    public void setData(String id){
        this.userID=id;
    }
    public String getData(){
        return this.userID;
    }

    public static synchronized Globals getInstance(){
        if(instance==null){
            instance=new Globals();
        }
        return instance;
    }
}