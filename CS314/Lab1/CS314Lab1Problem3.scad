module bookEnd(x, y, z, mirror)
{ 
		//If we want a mirrored book end, move the bookends around a bunch
		angle = (mirror == true) ? 180 : 0;
		moretranslate = (mirror == true) ? -50 : 0;
		lesstranslate = (mirror == false) ? -20 : 0;

		// Combines all of the shapes and angles them properly
		rotate([0,0,angle]) translate([x, y+moretranslate+lesstranslate, z]) union()
		{
			// Creates the vertical section of the bookend
			difference()
			{
				cube(size = [20, 1, 30], center = true);
				rotate([90,0,0]) translate([0,-2,0]) cube(size = [12, 25, 30], center = true);
			}
			
			// Book-facing end (bottom)
			translate([0,-5,-14.5]) cube(size = [20, 10, 1], center = true);
			// Brace end (bottom)
			translate([0,5,-14.5]) cube(size = [12, 15, 1], center = true);
		}
	
}

// Creates the two book ends
// left bookend
bookEnd(0,0,0,false);
// right bookend
bookEnd(0,30,0,true);