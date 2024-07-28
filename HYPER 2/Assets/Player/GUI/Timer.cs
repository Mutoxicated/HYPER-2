// using System;
// using TMPro;
// using UnityEngine;

// public class Timer : MonoBehaviour
// {
//     [SerializeField] private TMP_Text timer;

//     private int seconds = 0;
//     private int minutes = 0;
//     private int hours = 0;

//     private float time;

//     public void DStart() {
//         seconds = Save.rData.RunTimeSecs;

//         if (seconds >= 3600)
//             hours = Mathf.FloorToInt(Save.rData.RunTimeSecs/3600);
//         seconds -= hours*3600;
//         if (seconds >= 60)
//             minutes = Mathf.FloorToInt(Save.rData.RunTimeSecs/60);
//         seconds -= minutes*60;
//         Display();
//     }

//     private string StringifyValue(int num){
//         if (num < 10){
//             return "0"+num;
//         }else{
//             return num.ToString();
//         }
//     }

//     private void Display() {
//         if (hours > 0) {
//             timer.text = String.Format("{0}:{1}:{2}", StringifyValue(hours), StringifyValue(minutes), StringifyValue(seconds));
//         }else if (minutes > 0) {
//             timer.text = String.Format("{0}:{1}", StringifyValue(minutes), StringifyValue(seconds));
//         }else {
//             timer.text = String.Format("{0}", StringifyValue(seconds));
//         }
//     }

//     private void Update(){
//         time += Time.deltaTime;
//         if (time >= 1f){
//             Save.rData.IncRunTime();
//             seconds++;
//             if (seconds == 60){
//                 seconds = 0;
//                 minutes++;
//             }
//             if (minutes == 60) {
//                 minutes = 0;
//                 hours++;
//             }
//             timer.text = "";
//             Display();
//             time = 0f;
//         }
//     }
// }