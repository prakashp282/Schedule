package devine.prakash.com.geocollect;

public class User {
    String Name,VecicalNumber,MfgDt,Model,email,password;


     public User(String name, String vecicalNumber, String mfgDt, String model, String email, String password) {
        Name = name;
        VecicalNumber = vecicalNumber;
        MfgDt = mfgDt;
        Model = model;
        this.email = email;
        this.password = password;
    }

    public String getEmail() {
        return email;
    }

    public String getPassword() {
        return password;
    }

}
