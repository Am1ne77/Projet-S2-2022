
//var database;
//var firebase;
function setup()
{
  const firebaseConfig = {
    apiKey: "AIzaSyB9bsIclg8xXufRq_m8Bfa2WnKgHMqN4ZE",
    authDomain: "capagame-d54ad.firebaseapp.com",
    databaseURL: "https://capagame-d54ad-default-rtdb.europe-west1.firebasedatabase.app",
    projectId: "capagame-d54ad",
    storageBucket: "capagame-d54ad.appspot.com",
    messagingSenderId: "423648182646",
    appId: "1:423648182646:web:656008c021abdae4883a57",
    measurementId: "G-8P79PV2WLX"
  };


  const app = initializeApp(firebaseConfig);
  const data = getAnalytics(app);

var ref = database.ref('xps');
ref.on('value', gotData,errData);

}

function gotData(data){
  console.log(data.val());

}

function errData(err){
  console.log('Error!');
  console.log(err);

}