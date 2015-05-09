package com.jaredpetersen.hiccup;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;

import org.json.JSONObject;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.ArrayList;

/**
 * Created by jaredpetersen on 3/2/15.
 */
public class DatabaseFragment extends Fragment implements View.OnClickListener, AdapterView.OnItemClickListener
{

    TextView resultsTextView;
    EditText mainEditText;
    Button mainButton;
    ListView mainListView;
    JSONAdapter mJSONAdapter;
    ArrayList mGameList = new ArrayList();
    Globals g = Globals.getInstance();
    private static final String QUERY_URL = "http://www.wou.edu/~jpetersen11/api/videogame.php?key=R@inDr0psOnro53s?&search=";

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        View v = inflater.inflate(R.layout.database_fragment, container, false);

        // Search bar
        mainEditText = (EditText) v.findViewById(R.id.search_bar);
        mainButton = (Button) v.findViewById(R.id.search_button);
        mainButton.setOnClickListener(this);

        // No results found Text View
        // Do not display anything in this field when the app first starts up
        resultsTextView = (TextView) v.findViewById(R.id.no_results);

        // Access the ListView
        mainListView = (ListView) v.findViewById(R.id.database_listview);

        mJSONAdapter = new JSONAdapter(getActivity(), getActivity().getLayoutInflater());
        mainListView.setAdapter(mJSONAdapter);

        mainListView.setOnItemClickListener(this);

        return v;
    }

    public static DatabaseFragment newInstance()
    {
        DatabaseFragment f = new DatabaseFragment();
        Bundle b = new Bundle();

        f.setArguments(b);

        return f;
    }

    @Override
    public void onClick(View v)
    {
        // Received a search request, process the search on the database
        searchDatabase(mainEditText.getText().toString());
    }

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
        JSONObject jsonObject = (JSONObject) mJSONAdapter.getItem(position);
        String gameID = jsonObject.optString("videogameID","");
        String title = jsonObject.optString("title","");
        String releaseDate = jsonObject.optString("releaseDate","");
        String consoleName = jsonObject.optString("consoleName","");
        String esrb = jsonObject.optString("esrb","");
        String completionStatusID = jsonObject.optString("completionStatusID","");
        Log.e("hookamooka", completionStatusID);

        // create an Intent to take you over to a new DetailActivity
        Intent detailIntent = new Intent(getActivity(), VideogameDetail.class);

        // pack away the data about the cover
        // into your Intent before you head out
        detailIntent.putExtra("gameID", gameID);
        detailIntent.putExtra("title", title);
        detailIntent.putExtra("releaseDate", releaseDate);
        detailIntent.putExtra("consoleName", consoleName);
        detailIntent.putExtra("esrb", esrb);


        // start the next Activity using your prepared Intent
        startActivity(detailIntent);
    }

    private void searchDatabase(String searchString) {

        // Prepare the search string to be put in a URL
        String urlString = "";
        try
        {
            urlString = URLEncoder.encode(searchString, "UTF-8");
        }
        catch (UnsupportedEncodingException e)
        {
            e.printStackTrace();
            Toast.makeText(getActivity(), "Error: " + e.getMessage(), Toast.LENGTH_LONG).show();
        }

        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        Log.e("hookamooka", QUERY_URL + urlString);

        // Have the client get a JSONArray of data
        // and define how to respond
        client.get(QUERY_URL + urlString,
                new JsonHttpResponseHandler()
                {

                    @Override
                    public void onSuccess(JSONObject jsonObject)
                    {
                        // Reset the text back to nothing
                        resultsTextView.setText("");
                        // Check if the JSON has items in it
                        if (jsonObject.optJSONArray("results") != null)
                        {
                            if (jsonObject.optJSONArray("results").isNull(0)) {
                                // No items, let the user know
                                resultsTextView.setText("No Results Found");
                            } else {
                                // If items exist display them
                                // If not, empty out the ListView
                                mJSONAdapter.updateData(jsonObject.optJSONArray("results"));
                            }
                        }
                        else
                        {
                            resultsTextView.setText("No Results Found");
                        }
                    }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error)
                    {
                        // Display a "Toast" message
                        // to announce the failure
                        Toast.makeText(getActivity().getApplicationContext(), "Error: " + statusCode + " " + throwable.getMessage(), Toast.LENGTH_LONG).show();

                        // Log error message
                        // to help solve any problems
                        Log.e("tag", statusCode + " " + throwable.getMessage());
                    }
                });
    }
}