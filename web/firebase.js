
//var database;
//var firebase;
function setup()
{
}
var firebaseConfig = {
  apiKey: "AIzaSyB9bsIclg8xXufRq_m8Bfa2WnKgHMqN4ZE",
  authDomain: "capagame-d54ad.firebaseapp.com",
  databaseURL: "https://capagame-d54ad-default-rtdb.europe-west1.firebasedatabase.app",
  projectId: "capagame-d54ad",
  storageBucket: "capagame-d54ad.appspot.com",
  messagingSenderId: "423648182646",
  appId: "1:423648182646:web:656008c021abdae4883a57",
  measurementId: "G-8P79PV2WLX"
};


var app = firebase.initializeApp(firebaseConfig);


var database = firebase.database();

var ref = database.ref('users');


ref.on('value',gotData,errData);

console.log(ref);


function gotData(data){
  var a="";
  var scores = data.val();
  var keys =Object.keys(scores);
  var nomtableau=new Array();
  console.log(keys);
  for (var i=0; i<keys.length;i++){
    var k = keys[i];
    var name = scores[k].username;
    var xp =scores[k].xp;
    
    var xpPendu= scores[k].xpPendu;
    var xpPuissance4 =scores[k].xpPuissance4;
    
    var xpMiniTank =scores[k].xpMiniTank;
    
    nomtableau[i]={k,name,xp,xpPendu,xpPuissance4,xpMiniTank};
    // var td = createElement("th",i);
    // var td1 = createElement("th",username);
    // var tr = createElement('tr', td+td1 );
    // tr.parent('scoretabl')
  // a=a+"<tr>"+"<th>"+(i+1)+"</th>"+"<th>"+name+"</th>"+"<th>"+xp+"</th>"+"<th>"+xpPendu+"</th>"+"<th>"+xpPuissance4+"</th>"+"<th>"+xpMiniTank+"</th>"+"</tr>";

  }
  

  console.log(nomtableau);

  for (var i=0;i<keys.length;i++){
    
    var min_idx = i;
    for (var j=i;j<keys.length;j++){
      if (nomtableau[min_idx].xp < nomtableau[j].xp)
          { console.log (nomtableau[min_idx].xp, nomtableau[j].xp);
            min_idx = j;}
    }
    var tmp =nomtableau[min_idx];
    nomtableau[min_idx]= nomtableau[i];
    
    nomtableau[i]=tmp;
  }
  // nomtableau.xp.sort();
  const new_tbody = document.createElement('tbody');
  for (var i=0; i<nomtableau.length;i++){
    var row_1 = document.createElement('tr');
var heading_1 = document.createElement('td');
heading_1.innerHTML = i+1;
var heading_2 = document.createElement('td');
heading_2.innerHTML = nomtableau[i].name;
var heading_3 = document.createElement('td');
heading_3.innerHTML = nomtableau[i].xp;

var heading_4 = document.createElement('td');
heading_4.innerHTML = nomtableau[i].xpPendu;

var heading_5 = document.createElement('td');
heading_5.innerHTML = nomtableau[i].xpPuissance4;

var heading_6 = document.createElement('td');
heading_6.innerHTML = nomtableau[i].xpMiniTank;

row_1.appendChild(heading_1);
row_1.appendChild(heading_2);
row_1.appendChild(heading_3);
row_1.appendChild(heading_4);
row_1.appendChild(heading_5);
row_1.appendChild(heading_6);

new_tbody.appendChild(row_1);
   }
   
var old_tbody =document.getElementById('tbody');

old_tbody.parentNode.replaceChild(new_tbody, old_tbody)


  //document.write("<thread><tr><th>Position</th><th>UserName</th><th>Xp</th><th>XpPendu</th><th>XpPuissance4</th><th>XpMiniTanks</th></tr></thread>");
  //document.write("<table><tbody>"+a+"</table></tbody>");
}

function errData(err){
  console.log('Error!');
  console.log(err);

}
