// Initialize the Map details.
function initialize() {

    // make a new LatLong for the report's location
    var centerLatitude = 39.8282;
    var centerLongitude = -98.5795;
    var reportLocation = new google.maps.LatLng(centerLatitude, centerLongitude);

    // initialize map properties
    var mapProp = {
        center: reportLocation,
        zoom: 3,
        mapTypeId: google.maps.MapTypeId.ROADMAP // types include ROADMAP, SATELLITE, HYBRID, and TERRAIN
    };

    // get the map
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    var marker;

    // add click listener
    google.maps.event.addListener(map, 'click', function (event) {
        placeMarker(event.latLng);
        var form = document.forms['createForm'];
        form.elements["Latitude"].value = event.latLng.lat();
        form.elements["Longitude"].value = event.latLng.lng();
        //Console.log("Hello, world!");

    });

    function placeMarker(location) {
        // remove old marker
        if (marker != null || marker != undefined) {
            marker.setMap(null);
        }
        //  add a new marker
        marker = new google.maps.Marker({
            position: location,
            map: map,
        });

        // remove the current marker if it is there


        var infowindow = new google.maps.InfoWindow({
            content: 'Latitude: ' + location.lat() +
            '<br>Longitude: ' + location.lng()
        });
        infowindow.open(map, marker);
    }

}

// display the map
google.maps.event.addDomListener(window, 'load', initialize);