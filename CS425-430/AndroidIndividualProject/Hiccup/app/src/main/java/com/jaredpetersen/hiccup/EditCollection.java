package com.jaredpetersen.hiccup;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONObject;


public class EditCollection extends ActionBarActivity implements View.OnClickListener {

    Button markIncomplete;
    Button markBeat;
    Button markComplete;
    Button markRemove;
    Globals g = Globals.getInstance();
    String userID = g.getData();
    String gameID;
    String status;
    TextView statusTV;
    private static final String queryAddCollectionURL =
            "http://www.wou.edu/~jpetersen11/api/ownershiptwo.php";
    private static final String queryRemoveCollectionURl =
            "http://www.wou.edu/~jpetersen11/api/ownershipremove.php";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_collection);

        gameID = this.getIntent().getExtras().getString("gameID");
        status = this.getIntent().getExtras().getString("status");

        markIncomplete = (Button) findViewById(R.id.incomplete_button);
        markIncomplete.setOnClickListener(this);

        markBeat = (Button) findViewById(R.id.beat_button);
        markBeat.setOnClickListener(this);

        markComplete = (Button) findViewById(R.id.complete_button);
        markComplete.setOnClickListener(this);

        markRemove = (Button) findViewById(R.id.unowned_button);
        markRemove.setOnClickListener(this);

        statusTV = (TextView) findViewById(R.id.completion_status);
        statusTV.setText(status);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_add_collection, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onClick(View v)
    {
        switch (v.getId()) {

            case R.id.incomplete_button:
                addToCollection("1");
                statusTV.setText("Incomplete");
                g.setCollectionLoadedStatus(false);
                //g.getCollection().loadCollection();
                break;

            case R.id.beat_button:
                addToCollection("2");
                statusTV.setText("Beat");
                break;

            case R.id.complete_button:
                addToCollection("3");
                statusTV.setText("Complete");
                break;

            case R.id.unowned_button:
                removeFromCollection();
                statusTV.setText("Unowned");
                break;

            default:
                statusTV.setText("Unowned");
                break;
        }
    }

    private void addToCollection(String collectionStatus)
    {
        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        RequestParams params = new RequestParams();
        params.put("game", gameID);
        params.put("user", userID);
        params.put("status", collectionStatus);
        params.put("key", "R@inDr0psOnro53s?");

        // Have the client post data to the site
        client.post(queryAddCollectionURL, params,
                new JsonHttpResponseHandler() {
                    @Override
                    public void onSuccess(JSONObject jsonObject) { }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error) {
                        Log.e("hookamooka", "This was fail");
                    }
                });
    }

    private void removeFromCollection()
    {
        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        RequestParams params = new RequestParams();
        params.put("game", gameID);
        params.put("user", userID);
        params.put("key", "R@inDr0psOnro53s?");

        Log.e("hookamooka", "Remove From Collection");

        // Have the client post data to the site
        client.post(queryRemoveCollectionURl, params,
                new JsonHttpResponseHandler() {
                    @Override
                    public void onSuccess(JSONObject jsonObject) {
                    }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error) {
                        Log.e("hookamooka", "This was fail");
                    }
                });
    }
}
