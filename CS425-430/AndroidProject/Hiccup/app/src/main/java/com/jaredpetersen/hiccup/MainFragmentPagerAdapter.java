package com.jaredpetersen.hiccup;

import android.graphics.drawable.Drawable;
import android.support.v4.app.FragmentManager;
import android.content.Context;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentPagerAdapter;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.style.ImageSpan;

/**
 * Created by jaredpetersen on 3/2/15.
 */
public class MainFragmentPagerAdapter extends FragmentPagerAdapter
{
    final int PAGE_COUNT = 4;
    private String tabTitles[] = new String[] { "Database", "Collection", "Wishlist", "Metrics" };
    private Context context;

    private int[] imageResId = {
            R.drawable.ic_action_cloud,
            R.drawable.ic_action_gamepad,
            R.drawable.ic_action_view_as_list,
            R.drawable.ic_action_sort_by_size
    };

    public MainFragmentPagerAdapter(FragmentManager fm, Context context)
    {
        super(fm);
        this.context = context;
    }

    @Override
    public int getCount()
    {
        return PAGE_COUNT;
    }

    @Override
    public Fragment getItem(int position)
    {
        //return PageFragment.newInstance(position + 1);
        switch (position) {
            case 0: return DatabaseFragment.newInstance();
            case 1: return CollectionFragment.newInstance("My Collection");
            case 2: return WishlistFragment.newInstance("My Wishlist");
            case 3: return MetricsFragment.newInstance("Collection Metrics");
            default: return DatabaseFragment.newInstance();
        }
    }

    @Override
    public CharSequence getPageTitle(int position)
    {
        // Generate title based on item position
        // return tabTitles[position];
        Drawable image = context.getResources().getDrawable(imageResId[position]);
        image.setBounds(0, 0, image.getIntrinsicWidth(), image.getIntrinsicHeight());
        SpannableString sb = new SpannableString(" ");
        ImageSpan imageSpan = new ImageSpan(image, ImageSpan.ALIGN_BOTTOM);
        sb.setSpan(imageSpan, 0, 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        return sb;
    }
}
