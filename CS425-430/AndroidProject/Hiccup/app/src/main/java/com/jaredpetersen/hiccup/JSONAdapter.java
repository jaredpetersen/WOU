package com.jaredpetersen.hiccup;

import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Created by jaredpetersen on 3/7/15.
 */
public class JSONAdapter extends BaseAdapter
{

    Context mContext;
    LayoutInflater mInflater;
    JSONArray mJsonArray;

    public JSONAdapter(Context context, LayoutInflater inflater)
    {
        mContext = context;
        mInflater = inflater;
        mJsonArray = new JSONArray();
    }

    @Override
    public int getCount()
    {
        return mJsonArray.length();
    }

    @Override
    public JSONObject getItem(int position)
    {
        return mJsonArray.optJSONObject(position);
    }

    @Override
    public long getItemId(int position)
    {
        // Come back to this later
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent)
    {
        ViewHolder holder;

        // check if the view already exists
        // if so, no need to inflate and findViewById again!
        if (convertView == null)
        {

            // Inflate the custom row layout from your XML.
            convertView = mInflater.inflate(R.layout.row_game, null);

            // create a new "Holder" with subviews
            holder = new ViewHolder();
            holder.gameTitleTextView = (TextView) convertView.findViewById(R.id.game_title);
            holder.consoleNameTextView = (TextView) convertView.findViewById(R.id.console_name);
            // hang onto this holder for future recyclage
            convertView.setTag(holder);
        }

        else
        {

            // skip all the expensive inflation/findViewById
            // and just get the holder you already made
            holder = (ViewHolder) convertView.getTag();
        }

        JSONObject jsonObject = (JSONObject) getItem(position);

        String gameTitle = "";
        String consoleName = "";

        if (jsonObject.has("title"))
        {
            gameTitle = jsonObject.optString("title");
        }

        if (jsonObject.has("consoleName"))
        {
            consoleName = jsonObject.optString("consoleName");
        }

        // Send these Strings to the TextViews for display
        holder.gameTitleTextView.setText(gameTitle);
        holder.consoleNameTextView.setText('(' + consoleName + ')');

        return convertView;
    }

    public void updateData(JSONArray jsonArray)
    {
        // update the adapter's dataset
        mJsonArray = jsonArray;
        notifyDataSetChanged();
    }

    // this is used so you only ever have to do
    // inflation and finding by ID once ever per View
    private static class ViewHolder
    {
        public TextView gameTitleTextView;
        public TextView consoleNameTextView;
    }
}
