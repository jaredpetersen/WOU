package com.jaredpetersen.hiccup;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import com.github.mikephil.charting.charts.PieChart;
import com.github.mikephil.charting.data.ChartData;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.PieData;
import com.github.mikephil.charting.data.PieDataSet;
import com.github.mikephil.charting.utils.ColorTemplate;
import com.github.mikephil.charting.utils.ValueFormatter;
import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;

import org.json.JSONObject;

import java.util.ArrayList;

/**
 * Created by jaredpetersen on 3/2/15.
 */
public class MetricsFragment extends Fragment
{
    PieChart chart;
    String userID = ((MainActivity)getActivity()).userID;
    Globals g = Globals.getInstance();
    private static final String QUERY_URL = "http://www.wou.edu/~jpetersen11/api/ownershipfour.php?&key=R@inDr0psOnro53s?&user=";
    int completeCount = 0;
    int incompleteCount = 0;
    int beatCount = 0;
    int tempCount = 0;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        View v = inflater.inflate(R.layout.metrics_fragment, container, false);
        g.setMetrics(this);

        chart = (PieChart) v.findViewById(R.id.chart);
        chart.setCenterText("Completion Status");
        chart.setDescription("");
        setData();

        return v;
    }

    public static MetricsFragment newInstance(String text)
    {

        MetricsFragment f = new MetricsFragment();
        Bundle b = new Bundle();
        b.putString("msg", text);

        f.setArguments(b);

        return f;
    }

    public void setData()
    {
        // Y Values (collection data)
        ArrayList<Entry> yVals = new ArrayList<Entry>();

        beatCount=1;
        completeCount=2;
        incompleteCount=1;

        /*loadCollectionStats(0);
        //completeCount = tempCount;
        Log.d("hookamooka", "Status: " + 0);
        Log.d("hookamooka", "TempCount: " + tempCount);
        tempCount = 0;

        loadCollectionStats(1);
        //incompleteCount = tempCount;
        Log.d("hookamooka", "Status: " + 0);
        Log.d("hookamooka", "TempCount: " + tempCount);
        tempCount = 0;

        loadCollectionStats(2);
        //beatCount = tempCount;
        Log.d("hookamooka", "Status: " + 0);
        Log.d("hookamooka", "TempCount: " + beatCount);
        tempCount = 0;*/

        // Incomplete
        yVals.add(new Entry(incompleteCount, 0));
        // Beat
        yVals.add(new Entry(beatCount, 1));
        // Complete
        yVals.add(new Entry(completeCount, 2));

        ArrayList<String> xVals = new ArrayList<String>();
        xVals.add("Incomplete");
        xVals.add("Beat");
        xVals.add("Complete");

        PieDataSet dataset = new PieDataSet(yVals, "");
        dataset.setValueFormatter(new IntegerFormatter());

        // add colors
        dataset.setColors(ColorTemplate.VORDIPLOM_COLORS);

        PieData data = new PieData(xVals, dataset);
        chart.setData(data);
        chart.invalidate();
    }

    public void loadCollectionStats(int status)
    {
        // Create a client to perform networking
        AsyncHttpClient client = new AsyncHttpClient();

        Log.d("hookamooka", "URL: " + QUERY_URL + userID + "&status=" + status);

        // Have the client get a JSONArray of data
        // and define how to respond
        client.get(QUERY_URL + userID + "&status=" + status,
                new JsonHttpResponseHandler() {
                    @Override
                    public void onSuccess(JSONObject jsonObject) {
                        // Check if the JSON has items in it
                        if (jsonObject.optJSONArray("results") != null) {
                            Log.d("hookamooka", jsonObject.optJSONArray("results").toString());
                            if (jsonObject.optJSONArray("results").isNull(0))
                            {
                                beatCount = 0;
                            }
                            else
                            {
                                if (jsonObject.optJSONArray("results").optJSONObject(0) != null)
                                {
                                    Log.d("hookamooka", jsonObject.optJSONArray("results").optJSONObject(0).toString());
                                    Log.d("hookamooka", jsonObject.optJSONArray("results").optJSONObject(0).optString("gameCount"));
                                    beatCount = Integer.parseInt(jsonObject.optJSONArray("results").optJSONObject(0).optString("gameCount"));
                                }
                                else
                                {
                                    beatCount = 0;
                                }
                            }
                        } else {
                            beatCount = 0;
                        }

                        Log.d("hookamooka", "Temp: " + tempCount);

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

}

class IntegerFormatter implements ValueFormatter {

    @Override
    public String getFormattedValue(float value) {
        return "" + ((int) value);
    }
}