package com.jaredpetersen.hiccup;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.ActionBarActivity;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Collection;

/**
 * Created by jaredpetersen on 4/12/15.
 */

public class VideogameDetail extends ActionBarActivity implements View.OnClickListener {

    Button addToCollection;
    Globals g = Globals.getInstance();
    String userID = g.getData();
    String gameID;
    String consoleName;
    String gameTitle;
    String releaseDate;
    String esrb;
    TextView completionStatusTV;
    private static final String QUERY_URL = "http://www.wou.edu/~jpetersen11/api/ownershipthree.php?key=R@inDr0psOnro53s?&user=";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Tell the activity which XML layout is right
        setContentView(R.layout.videogame_detail);

        // Enable the "Up" button for more navigation options
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        // Only checking if gameID is null because the others will be null also if gameID is null
        // Have to use this check because hitting the back button on AddCollection will call
        // onCreate, which will break everything if it does all of this stuff
        if (gameID == null) {
            gameID = this.getIntent().getExtras().getString("gameID");
            gameTitle = this.getIntent().getExtras().getString("title");
            consoleName = this.getIntent().getExtras().getString("consoleName");
            releaseDate = this.getIntent().getExtras().getString("releaseDate");

            // Need to convert release date format
            SimpleDateFormat databaseFormat = new SimpleDateFormat("yyyy-MM-dd");
            SimpleDateFormat displayFormat = new SimpleDateFormat("MM/dd/yyyy");

            try {
                releaseDate = displayFormat.format(databaseFormat.parse(releaseDate));
            } catch (ParseException e) {
                e.printStackTrace();
            }

            esrb = this.getIntent().getExtras().getString("esrb");
        }

        Log.e("hookamooka", "Videogame Detail gameID = " + gameID);

        TextView titleTV = (TextView) findViewById(R.id.game_title);
        titleTV.setText(gameTitle);

        TextView consoleTV = (TextView) findViewById(R.id.game_console);
        consoleTV.setText(consoleName);

        TextView releaseTV = (TextView) findViewById(R.id.release_date);
        releaseTV.setText(releaseDate);

        TextView esrbTV = (TextView) findViewById(R.id.esrb);
        esrbTV.setText(esrb);

        getCompletionStatus();

        completionStatusTV = (TextView) findViewById(R.id.completion_status);

        addToCollection = (Button) findViewById(R.id.add_collection_button);
        addToCollection.setOnClickListener(this);

        // Access the imageview from XML
        //ImageView imageView = (ImageView) findViewById(R.id.img_cover);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }
        else if (id == R.id.action_logout) {
            // This will delete the stored credentials and return to the login activity
            finish();
            //getParent().finish();
            Intent i = new Intent(VideogameDetail.this, LoginActivity.class);
            startActivity(i);
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onClick(View v) {
        // create an Intent to take you over to a new AddCollection activity
        Intent collectionIntent = new Intent(this, AddCollection.class);
        collectionIntent.putExtra("gameID", gameID);
        // Definitely not the best practice, but it's 2:39 AM on a Monday, so...
        collectionIntent.putExtra("status", completionStatusTV.getText());
        startActivity(collectionIntent);

        //addToCollection();
        // Refresh the collection
        g.getCollection().loadCollection();
    }

    private void getCompletionStatus() {

        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        Log.e("hookamooka", QUERY_URL + userID + "&game=" + gameID);

        // Have the client get a JSONArray of data
        // and define how to respond
        client.get(QUERY_URL + userID + "&game=" + gameID,
                new JsonHttpResponseHandler() {

                    @Override
                    public void onSuccess(JSONObject jsonObject) {
                        // Check if the JSON has items in it
                        if (jsonObject.optJSONArray("results") != null)
                        {
                            completionStatusTV.setText(jsonObject.optJSONArray("results").optJSONObject(0).optString("status"));
                        }
                        else
                        {
                            completionStatusTV.setText("Unowned");
                        }
                    }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error) {
                        // Log error message
                        Log.e("hookamooka", statusCode + " " + throwable.getMessage());
                    }
                });
    }
}
