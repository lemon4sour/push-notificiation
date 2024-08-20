const firebaseConfig = {
  apiKey: "AIzaSyAutIv3u12-CKgs-pWN-RzAjju1rr2sWlc",
  authDomain: "push-notification-29e4d.firebaseapp.com",
  projectId: "push-notification-29e4d",
  storageBucket: "push-notification-29e4d.appspot.com",
  messagingSenderId: "603196456061",
  appId: "1:603196456061:web:022da529cdbb8dc0270514",
  databaseURL: "https://push-notification-29e4d-default-rtdb.europe-west1.firebasedatabase.app/"
};
const app = firebase.initializeApp(firebaseConfig)
const messaging = firebase.messaging()
const database = firebase.database()

document.getElementById("activate-notification").onclick = function(event) {
  event.preventDefault();


  messaging.getToken({ vapidKey: "BOVWtxkIT497lAvQFYW3ra-UzG0VpdOn9lMJX5SrizsQ5rZOO7RZEGbTPq7nmZQKp2cOs33-U66twnl8BzupRmo" })
  .then((token) => {
    if (token) {
      console.log(token);

      database.ref('tokens/' + token).set({
        timestamp: Date.now(),
        userAgent: navigator.userAgent,
      })
      .then(() => {
        console.log('Token sent to database successfully.');
      })
      .catch((error) => {
        console.error('Error sending token to database:', error);
      });
    }
  })
  .catch((err) => {
    console.error("An error occured when recieving token.");
  })
  

  messaging.onMessage((payload) => {
    console.log('Message received ', payload);
    const messagesElement = document.querySelector('.message')
    const dataHeaderElement = document.createElement('h5')
    const dataElement = document.createElement('pre')
    dataElement.style = "overflow-x: hidden;"
    dataHeaderElement.textContent = "Message Received:"
    dataElement.textContent = JSON.stringify(payload, null, 2)
    messagesElement.appendChild(dataHeaderElement)
    messagesElement.appendChild(dataElement)
  })
};

