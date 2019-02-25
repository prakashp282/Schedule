package devine.prakash.com.geocollect;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.LocationManager;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.FirebaseApp;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

public class MainActivity extends AppCompatActivity {

    private EditText inputEmail, inputPassword;
    private Button btnSignIn, btnSignUp;
    private ProgressBar progressBar;
    private FirebaseAuth auth;
    private FirebaseAuth.AuthStateListener authListener;
    DatabaseReference userDatabase;
    FirebaseUser currentUser;
    User CU;
    private static final int PERMISSIONS_REQUEST = 1;
    public EditText emailText, passwordText, userText, VechNoText, modelText, mfgDtText;



    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);


        // Check GPS is enabled
        LocationManager lm = (LocationManager) getSystemService(LOCATION_SERVICE);
        if (!lm.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
            Toast.makeText(this, "Please enable location services", Toast.LENGTH_SHORT).show();
            finish();
        }

        // Check location permission is granted - if it is, not granted
        // request the permission
        int permission = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_FINE_LOCATION);
        if (permission != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                    PERMISSIONS_REQUEST);
        }


        FirebaseApp.initializeApp(this);
        //Get Firebase auth instance
        auth = FirebaseAuth.getInstance();

        if (auth.getCurrentUser() != null) {
            startTrackerService();
            finish();
        }

        setContentView(R.layout.activity_main);

        btnSignIn = (Button) findViewById(R.id.Login);
        btnSignUp = (Button) findViewById(R.id.Register);
        emailText =(EditText) findViewById(R.id.email);
        userText = (EditText) findViewById(R.id.User);
        VechNoText =(EditText) findViewById(R.id.vechNo);
        modelText =(EditText) findViewById(R.id.model);
        mfgDtText = (EditText) findViewById(R.id.mfgDt);
        passwordText = (EditText) findViewById(R.id.password);
        progressBar = (ProgressBar) findViewById(R.id.login_pb);


        btnSignIn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this, LoginActivity.class));

            }
        });

        btnSignUp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                SIGNUP();
            }
        });
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[]
            grantResults) {
        if (!(requestCode == PERMISSIONS_REQUEST && grantResults.length == 1
                && grantResults[0] == PackageManager.PERMISSION_GRANTED)) {
            finish();
        }
    }




    void SIGNUP() {
        {
            //String name, String vecicalNumber, String mfgDt, String model, String email, String password

            CU = new User(userText.getText().toString().trim(), VechNoText.getText().toString().trim(), mfgDtText.getText().toString().trim(), modelText.getText().toString().trim(), emailText.getText().toString().trim(), passwordText.getText().toString().trim());

            String user=userText.getText().toString();
            String email=emailText.getText().toString();
            String pass=passwordText.getText().toString();
            String vechno=VechNoText.getText().toString();
            String mfg=mfgDtText.getText().toString();
            String model=modelText.getText().toString();


            if(pass.isEmpty()||user.isEmpty()||model.isEmpty()||mfg.isEmpty()||vechno.isEmpty()||email.isEmpty())
            {    Toast.makeText(MainActivity.this, R.string.complete_form_warning, Toast.LENGTH_LONG).show();

            } else if (CU.getPassword().toString().length() < 6) {
                //If password size is less than zero then dont accept
                Toast.makeText(MainActivity.this, R.string.password_len, Toast.LENGTH_LONG).show();

            } else {

                progressBar.setVisibility(View.VISIBLE);
                //create user
                auth.createUserWithEmailAndPassword(CU.getEmail(), CU.getPassword())
                        .addOnCompleteListener(MainActivity.this, new OnCompleteListener<AuthResult>() {
                            @Override
                            public void onComplete(@NonNull Task<AuthResult> task) {
                                Toast.makeText(MainActivity.this, "createUserWithEmail:onComplete:" + task.isSuccessful(), Toast.LENGTH_SHORT).show();
                                // If sign in fails, display a message to the user. If sign in succeeds
                                // the auth state listener will be notified and logic to handle the
                                // signed in user can be handled in the listener.
                                if (!task.isSuccessful()) {
                                    Toast.makeText(MainActivity.this, "Authentication failed." + task.getException(),
                                            Toast.LENGTH_SHORT).show();
                                } else {
                                    //successfullycreated user now login

                                    //authenticate user
                                    auth.signInWithEmailAndPassword(CU.getEmail(), CU.getPassword())
                                            .addOnCompleteListener(MainActivity.this, new OnCompleteListener<AuthResult>() {
                                                @Override
                                                public void onComplete(@NonNull Task<AuthResult> task) {
                                                    progressBar.setVisibility(View.GONE);

                                                    //loged in now write to data base
                                                    currentUser = auth.getCurrentUser();
                                                    Toast.makeText(getApplicationContext(),"loged in",Toast.LENGTH_LONG).show();
                                                    String uid = currentUser.getUid();
                                                    String path= "User/" + uid;//userText.getText().toString().trim();
                                                    userDatabase= FirebaseDatabase.getInstance().getReference(path);
                                                    userDatabase.setValue(CU);
                                                    startTrackerService();

                                                }
                                            });
                                }





                            }
                        });

            }
        }
    }


    private void startTrackerService(){
        startService(new Intent(this, TrackingService.class));
        finish();
    }





}





   /*

    @Override
    protected void onStart() {
        super.onStart();

        currentUser = mAuth.getCurrentUser();
        if (currentUser == null) {      //if not registered

            emailText =(EditText) findViewById(R.id.email);
            userText = (EditText) findViewById(R.id.User);
            VechNoText =(EditText) findViewById(R.id.vechNo);
            modelText =(EditText) findViewById(R.id.model);
            mfgDtText = (EditText) findViewById(R.id.mfgDt);
            passwordText = (EditText) findViewById(R.id.password);


            pb = (ProgressBar) findViewById(R.id.login_pb);

            setContentView(R.layout.activity_main);

        } else {
            startTrackerService();
        }

    }


void SIGNUP(){
    String user=userText.getText().toString();
    String email=emailText.getText().toString();
    String pass=passwordText.getText().toString();
    String vechno=VechNoText.getText().toString();
    String mfg=mfgDtText.getText().toString();
    String model=modelText.getText().toString();

    //String name, String vecicalNumber, String mfgDt, String model, String email, String password
//    CU = new User(userText.getText().toString().trim(),VechNoText.getText().toString().trim(),mfgDtText.getText().toString().trim(),modelText.getText().toString().trim(),emailText.getText().toString().trim(),passwordText.getText().toString().trim());


    Log.d(TAG,"user :" + user + vechno + mfg + model + email + pass);
    CU=new User(user,vechno,mfg,model,email,pass);

    //if(CU.getPassword().isEmpty()||CU.getEmail().isEmpty()||CU.getName().isEmpty()||CU.getMfgDt().isEmpty()||CU.getModel().isEmpty()||CU.getVecicalNumber().isEmpty())
    if(pass.isEmpty()||user.isEmpty()||model.isEmpty()||mfg.isEmpty()||vechno.isEmpty()||email.isEmpty())
    {
        Toast.makeText(MainActivity.this,R.string.complete_form_warning,Toast.LENGTH_LONG).show();

    }
    //else if(CU.getPassword().toString().length()<6)
    else if (pass.length()<6)
    {
        //If password size is less than zero then dont accept
        Toast.makeText(MainActivity.this,R.string.password_len,Toast.LENGTH_LONG).show();

    }
    else
    {

        pb.setVisibility(View.VISIBLE);
        //create user
        mAuth.createUserWithEmailAndPassword(CU.getEmail(), CU.getPassword())
                .addOnCompleteListener(MainActivity.this, new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        Toast.makeText(MainActivity.this, "createUserWithEmail:onComplete:" + task.isSuccessful(), Toast.LENGTH_SHORT).show();
                        pb.setVisibility(View.GONE);
                        // If sign in fails, display a message to the user. If sign in succeeds
                        // the auth state listener will be notified and logic to handle the
                        // signed in user can be handled in the listener.

                        if (!task.isSuccessful()) {
                            Toast.makeText(MainActivity.this,R.string.account_failed,Toast.LENGTH_LONG).show();
                            finish();
                        } else {
                            Toast.makeText(MainActivity.this, R.string.account_success, Toast.LENGTH_LONG).show();
                            finish();
                        }

                    }
                });

        mAuth.signInWithEmailAndPassword(CU.getEmail(), CU.getPassword())
                .addOnCompleteListener(MainActivity.this, new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        // If sign in fails, display a message to the user. If sign in succeeds
                        // the auth state listener will be notified and logic to handle the
                        // signed in user can be handled in the listener.
                        pb.setVisibility(View.GONE);
                        if (task.isSuccessful()) {
                            Toast.makeText(getApplicationContext(),"loged in",Toast.LENGTH_LONG).show();
                            String uid = currentUser.getUid();
                            String path= "User/"+uid;//userText.getText().toString().trim();
                            userDatabase= FirebaseDatabase.getInstance().getReference(path);
                            userDatabase.setValue(CU);
                            startTrackerService();
                        }
                    }
                });



    }

}
*/
