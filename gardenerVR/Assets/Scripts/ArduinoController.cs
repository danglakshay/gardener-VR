using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    [SerializeField] public string portName = "COM4"; // Change this to match your Arduino port
    [SerializeField] public int baudRate = 9600;
    
    private SerialPort serialPort;
    private bool isConnected = false;
    
    void Start()
    {
        ConnectToArduino();
    }
    
    void OnDestroy()
    {
        DisconnectFromArduino();
    }
    
    private void ConnectToArduino()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 100;
            isConnected = true;
            Debug.Log("Connected to Arduino on " + portName);
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting to Arduino: " + e.Message);
        }
    }
    
    private void DisconnectFromArduino()
    {
        if (isConnected && serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            isConnected = false;
            Debug.Log("Disconnected from Arduino");
        }
    }
    
    // Call this method when you want to move a servo
    public void MoveServo(int servoId, int position)
    {
        if (!isConnected || serialPort == null)
        {
            Debug.LogWarning("Arduino not connected");
            return;
        }
        
        // Format the command string
        string command = $"S:{servoId}:{position}\n";
        
        try
        {
            serialPort.WriteLine(command);
            Debug.Log($"Command sent: {command}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending command to Arduino: " + e.Message);
            DisconnectFromArduino();
        }
    }
}