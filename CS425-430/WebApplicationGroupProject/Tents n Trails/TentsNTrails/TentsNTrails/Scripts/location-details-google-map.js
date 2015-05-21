// Initialize the Map details.
function initialize() {

    // make a new LatLong for the report's location
    var reportLocation = new google.maps.LatLng(@Model.Latitude, @Model.Longitude);

    // initialize map properties
    var mapProp = {
        center: reportLocation,
        zoom: 7,
        mapTypeId: google.maps.MapTypeId.ROADMAP // types include ROADMAP, SATELLITE, HYBRID, and TERRAIN
    };

    // get the map
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

    // add a marker to the map
    var marker = new google.maps.Marker({position: reportLocation, map: map});
    var infowindow = new google.maps.InfoWindow({content: 'Loading...'});
    marker.html='<div class="label"  ><a>@Model.Label</a></div>';
    google.maps.event.addListener(marker, 'click', function() {infowindow.setContent(this.html); infowindow.open(map,this);});
}

// display the map
google.maps.event.addDomListener(window, 'load', initialize);