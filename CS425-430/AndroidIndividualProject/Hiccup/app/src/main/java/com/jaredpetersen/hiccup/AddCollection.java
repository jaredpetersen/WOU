package com.jaredpetersen.hiccup;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONObject;


public class AddCollection extends ActionBarActivity implements View.OnClickListener {

    Button markIncomplete;
    Button markBeat;
    Button markComplete;
    Globals g = Globals.getInstance();
    String userID = g.getData();
    String gameID;
    private static final String QUERY_URL = "http://www.wou.edu/~jpetersen11/api/ownershiptwo.php";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_collection);

        gameID = this.getIntent().getExtras().getString("gameID");

        markIncomplete = (Button) findViewById(R.id.incomplete_button);
        markIncomplete.setOnClickListener(this);

        markBeat = (Button) findViewById(R.id.beat_button);
        markBeat.setOnClickListener(this);

        markComplete = (Button) findViewById(R.id.complete_button);
        markComplete.setOnClickListener(this);
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
    public void onClick(View v) {

    }

    private void addToCollection()
    {
        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        RequestParams params = new RequestParams();
        params.put("game", gameID);
        params.put("user", userID);
        params.put("key", "R@inDr0psOnro53s?");

        // Have the client post data to the site
        client.post(QUERY_URL, params,
                new JsonHttpResponseHandler() {
                    @Override
                    public void onSuccess(JSONObject jsonObject) { }

                    @Override
                    public void onFailure(int statusCode, Throwable throwable, JSONObject error) {
                        Log.e("hookamooka", "This was fail");
                    }
                });
    }
}
