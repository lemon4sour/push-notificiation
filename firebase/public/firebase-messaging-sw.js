importScripts('https://www.gstatic.com/firebasejs/9.14.0/firebase-app-compat.js')
importScripts('https://www.gstatic.com/firebasejs/9.14.0/firebase-messaging-compat.js')

const firebaseConfig = {
  apiKey: "AIzaSyAutIv3u12-CKgs-pWN-RzAjju1rr2sWlc",
  authDomain: "push-notification-29e4d.firebaseapp.com",
  projectId: "push-notification-29e4d",
  storageBucket: "push-notification-29e4d.appspot.com",
  messagingSenderId: "603196456061",
  appId: "1:603196456061:web:022da529cdbb8dc0270514"
};
const app = firebase.initializeApp(firebaseConfig)
const messaging = firebase.messaging()

messaging.onBackgroundMessage(messaging, (payload) => {
  console.log('[firebase-messaging-sw.js] Received background message ', payload);


  const notificationTitle = 'Background Message Title';
  const notificationOptions = {
    body: 'Background Message body.',
    icon: '/firebase-logo.png'
  };

  self.registration.showNotification(notificationTitle,
    notificationOptions);
});
