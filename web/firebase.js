
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


  
firebase.initializeApp(firebaseConfig);
database = firebase.database();
console.log(database);

var ref = database.ref('xp');
ref.on('valus', gotData,errData);

function gotData(data){
  console.log(data);

}

function errData(err){
  console.log('Error!');
  console.log(err);

}