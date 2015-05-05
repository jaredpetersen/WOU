package com.jaredpetersen.hiccup;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;

import org.json.JSONObject;

import java.util.ArrayList;

/**
 * Created by jaredpetersen on 3/2/15.
 */
public class CollectionFragment extends Fragment implements AdapterView.OnItemClickListener {
    TextView resultsTextView;
    EditText mainEditText;
    Button mainButton;
    ListView mainListView;
    JSONAdapter mJSONAdapter;
    ArrayList mGameList = new ArrayList();
    String userID = ((MainActivity)getActivity()).userID;
    Globals g = Globals.getInstance();
    private static final String QUERY_URL = "http://www.wou.edu/~jpetersen11/api/ownership.php?&key=R@inDr0psOnro53s?&user=";

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        View v = inflater.inflate(R.layout.collection_fragment, container, false);

        resultsTextView = (TextView) v.findViewById(R.id.collection_no_results);
        resultsTextView.setText("Loading... Please wait.");

        // Access the ListView
        mainListView = (ListView) v.findViewById(R.id.collection_listview);

        mJSONAdapter = new JSONAdapter(getActivity(), getActivity().getLayoutInflater());
        mainListView.setAdapter(mJSONAdapter);

        mainListView.setOnItemClickListener(this);

        loadCollection();

        g.setCollection(this);

        return v;
    }

    public static CollectionFragment newInstance(String text)
    {

        CollectionFragment f = new CollectionFragment();
        Bundle b = new Bundle();

        f.setArguments(b);

        return f;
    }

    public void loadCollection()
    {
        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        // Have the client get a JSONArray of data
        // and define how to respond
        client.get(QUERY_URL + userID,
                new JsonHttpResponseHandler() {
                    @Override
                    public void onSuccess(JSONObject jsonObject) {
                        // Check if the JSON has items in it
                        if (jsonObject.optJSONArray("results") != null) {
                            if (jsonObject.optJSONArray("results").isNull(0)) {
                                // No items, let the user know
                                resultsTextView.setText("You do not own any games.");
                            }
                            else
                            {
                                resultsTextView.setText("");
                                // If items exist display them
                                // If not, empty out the ListView
                                mJSONAdapter.updateData(jsonObject.optJSONArray("results"));
                            }
                        }
                        else
                        {
                            resultsTextView.setText("You do not own any games.");
                        }

                    }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error) {
                        // Display a "Toast" message
                        // to announce the failure
                        Toast.makeText(getActivity().getApplicationContext(), "Error: " + statusCode + " " + throwable.getMessage(), Toast.LENGTH_LONG).show();

                        // Log error message
                        // to help solve any problems
                        Log.e("tag", statusCode + " " + throwable.getMessage());
                    }
                });
    }

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
        JSONObject jsonObject = (JSONObject) mJSONAdapter.getItem(position);
        String title = jsonObject.optString("title","");
        String releaseDate = jsonObject.optString("releaseDate","");
        String consoleName = jsonObject.optString("consoleName","");
        String esrb = jsonObject.optString("esrb","");

        // create an Intent to take you over to a new DetailActivity
        Intent detailIntent = new Intent(getActivity(), VideogameDetail.class);

        // pack away the data about the cover
        // into your Intent before you head out
        detailIntent.putExtra("title", title);
        detailIntent.putExtra("releaseDate", releaseDate);
        detailIntent.putExtra("consoleName", consoleName);
        detailIntent.putExtra("esrb", esrb);

        // start the next Activity using your prepared Intent
        startActivity(detailIntent);
    }
}