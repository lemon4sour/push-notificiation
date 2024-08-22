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

console.log("test message 1");

function getTopics() {
  const userAgent = navigator.userAgent;

  let browserTopic;
  let systemTopic;
  let hardwareTopic;

  if (userAgent.includes("Firefox")) {
    browserTopic = "firefox";
  } else if (userAgent.includes("Chrome")) {
    browserTopic = "chrome";
  } else if (userAgent.includes("Opera")) {
    browserTopic = "opera";
  } else if (userAgent.includes("Safari")) {
    browserTopic = "safari";
  } else {
    browserTopic = "other_browser";
  }

  if (userAgent.includes("Mobile")) {
    hardwareTopic = "mobile";
  } else if (userAgent.includes("Windows NT") || userAgent.includes("Macintosh")) {
    hardwareTopic = "desktop";
  } else {
    hardwareTopic = "other_device";
  }

  if (userAgent.includes("Windows NT")) {
    systemTopic = "windows";
  } else if (userAgent.includes("Mobile") || userAgent.includes("Android")) {
    systemTopic = "android";
  } else if (userAgent.includes("iPhone") || userAgent.includes("iPad")) {
    systemTopic = "ios";
  } else if (userAgent.includes("Macintosh") || userAgent.includes("Mac OS")) {
    systemTopic = "mac";
  } else if (userAgent.includes("Linux")) {
    systemTopic = "linux";
  } else {
    systemTopic = "other_os";
  }
  return [browserTopic, systemTopic, hardwareTopic];
}

console.log("test message 2");

document.getElementById("activate-notification").onclick = async function(event) {

  console.log("activated");
  document.getElementById("test").textContent = "Clicked on button..";

  const name = document.getElementById('name').value;

  document.getElementById("test").textContent = "Clicked on button...";

  const result = await Notification.requestPermission();
  document.getElementById("test").textContent = "Clicked on button....";
  document.getElementById("test").textContent = "the notif request got "+result;

  if (result === "denied") return;
  

  messaging.getToken({ vapidKey: "BOVWtxkIT497lAvQFYW3ra-UzG0VpdOn9lMJX5SrizsQ5rZOO7RZEGbTPq7nmZQKp2cOs33-U66twnl8BzupRmo" })
  .then((token) => {
    if (token) {
      console.log(token);
      document.getElementById("test").textContent = token;

      database.ref('tokens/' + token).set({
        name: name || "",
        timestamp: Date.now(),
        userAgent: navigator.userAgent,
        topics: getTopics(),
      })
      .then(() => {
        console.log('Token sent to database successfully.');
        document.getElementById("test").textContent = "sent " + token;
      })
      .catch((error) => {
        console.error('Error sending token to database:', error);
      });
    }
  })
  .catch((err) => {
    console.error("An error occurred when receiving the token.", err);
  });

  messaging.onMessage((payload) => {
    console.log('Message received ', payload);
    const messagesElement = document.querySelector('.message');
    const dataHeaderElement = document.createElement('h5');
    const dataElement = document.createElement('pre');
    dataElement.style = "overflow-x: hidden;";
    dataHeaderElement.textContent = "Message Received:";
    dataElement.textContent = JSON.stringify(payload, null, 2);
    messagesElement.appendChild(dataHeaderElement);
    messagesElement.appendChild(dataElement);
  });
};