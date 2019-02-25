package devine.prakash.com.geocollect;

import java.time.LocalDateTime;
import java.util.Date;
import java.util.Locale;

public class geolocation {

    String date;
    Double Latitude,Longitude;
    float Speed,Accuracy;

    public geolocation(String date, float accuracy, Double latitude, Double longitude, float speed) {
        this.date = date;
        Accuracy = accuracy;
        Latitude = latitude;
        Longitude = longitude;
        Speed = speed;
    }
}
