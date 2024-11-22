using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Adapt;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class DashboardController : MonoBehaviour
{
    private VisualElement outputContainer;
    private TextField inputField;
    private ScrollView scrollView;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;
    private VisualElement sensorGraphs;
    private bool isExpanded = false;
    private Label lastUpdatedLabel;
    private DateTime lastUpdatedDateTime;
    private CustomUI.ToggleButton refreshButton;
    private StatePublisher statePublisher;





    async void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        statePublisher = new StatePublisher();
        // Example: Setting values dynamically
        //countdownLabel.text = "05:55:25";
        //distanceLabel.text = "245 KM";
        //speedLabel.text = "25,555 KM/H";

        // Get references to UI elements
        outputContainer = root.Q<VisualElement>("terminal-output-container");
        inputField = root.Q<TextField>("terminal-input");
        scrollView = root.Q<ScrollView>("terminal-output");

        lastUpdatedLabel = root.Q<Label>("update-window-text");
        refreshButton = root.Q<CustomUI.ToggleButton>("refresh-button");
        refreshButton.SetOnClickBehavior(async () =>
        {
            await UpdateValues();
        });



        statePublisher.register(root.Q<CustomUI.Odometer>("instrument_temp"));
        statePublisher.register(root.Q<CustomUI.Odometer>("sipm_a_temp"));
        statePublisher.register(root.Q<CustomUI.Odometer>("sipm_b_temp"));

        statePublisher.register(root.Q<CustomUI.TimeChart>("altitude"));


        statePublisher.register(root.Q<CustomUI.Indicator>("starlink-status"));
        statePublisher.register(root.Q<CustomUI.Indicator>("tdrss-status"));
        statePublisher.register(root.Q<CustomUI.Indicator>("openport-status"));


        statePublisher.register(root.Q<CustomUI.HeadingInstrument>("heading_instrument"));

        //statePublisher.register(root.Q<CustomUI.AltitudeInstrument>("altitude_instrument"));


        // Register input handler
        inputField.RegisterCallback<KeyDownEvent>(OnKeyDown);

        //Debug.Log("Input Field registered");

        // Initial welcome message
        WriteLine("Terminal Emulator v1.0");
        WriteLine("Type 'help' for available commands");

        // Find chart container
        //var chartContainer = root.Q<VisualElement>("chart-container");

        // Create and add chart
        //XChartElement chartElement = new XChartElement();
        //chartContainer.Add(chartElement);

        // Add sample data
        //var data = new float[] { 10, 20, 15, 25, 18 };
        //var labels = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri" };
        //chartElement.AddData("Sample Series", data, labels);
        // Create the chart

        await GetUpdatedStateAtTimeInterval();


        // Customize appearance (optional)
    }

    // Update method to periodically refresh data if needed

    private void PrintClickMessage(ClickEvent evt)
    {
        Debug.Log($" Click registered");
    }


    private void ToggleExpanded()
    {
        isExpanded = !isExpanded;
        if (isExpanded)
        {
            sensorGraphs.AddToClassList("sensor-graphs--expanded");
            // Optionally bring to front in the visual hierarchy
            sensorGraphs.BringToFront();
        }
        else
        {
            sensorGraphs.RemoveFromClassList("sensor-graphs--expanded");
        }
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        Debug.Log(evt.keyCode);
        switch (evt.keyCode)
        {
            case KeyCode.Return:
                ProcessCommand();
                break;
            case KeyCode.UpArrow:
                NavigateHistory(-1);
                evt.StopPropagation();
                break;
            case KeyCode.DownArrow:
                NavigateHistory(1);
                evt.StopPropagation();
                break;
            default:
                Debug.Log("Idk");
                break;
        }
    }

    private void ProcessCommand()
    {
        string command = inputField.value.Trim();

        if (string.IsNullOrEmpty(command))
            return;

        // Echo command
        WriteLine($"> {command}", "terminal-line");

        // Add to history
        commandHistory.Add(command);
        historyIndex = commandHistory.Count;

        // Process command
        ExecuteCommand(command);

        // Clear input
        inputField.value = "";

        // Scroll to bottom
        //scrollView.ScrollTo(scrollView);
    }

    private void ExecuteCommand(string command)
    {
        // Simple command processing example
        switch (command.ToLower())
        {
            case "help":
                WriteLine("Available commands:", "terminal-line");
                WriteLine("  help - Show this help", "terminal-line");
                WriteLine("  clear - Clear terminal", "terminal-line");
                WriteLine("  echo [text] - Echo text", "terminal-line");
                break;

            case "clear":
                ClearTerminal();
                break;

            default:
                if (command.StartsWith("echo "))
                {
                    string text = command.Substring(5);
                    WriteLine(text, "terminal-line--success");
                }
                else
                {
                    WriteLine($"Command not found: {command}", "terminal-line--error");
                }
                break;
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0)
            return;

        historyIndex = Mathf.Clamp(historyIndex + direction, 0, commandHistory.Count);

        if (historyIndex == commandHistory.Count)
            inputField.value = "";
        else
            inputField.value = commandHistory[historyIndex];

        inputField.SelectAll();
    }

    public void WriteLine(string text, string className = "terminal-line")
    {
        var line = new Label(text);
        line.AddToClassList(className);
        outputContainer.Add(line);
    }

    private void ClearTerminal()
    {
        outputContainer.Clear();
    }

    public async Task UpdateValues()
    {
        await statePublisher.getState();

        statePublisher.notifyObserver();
        lastUpdatedDateTime = DateTime.Now;
    }

    public async Task GetUpdatedStateAtTimeInterval()
    {
        while (true)
        {
            await UpdateValues();

            await Task.Delay(3000);
        }
    }

    public void Update()
    {
        if(lastUpdatedDateTime == null)
        {
            Debug.Log("YOOOO");
            lastUpdatedLabel.text = "Not updated since start up";
            return;
        }
        lastUpdatedLabel.text = "Updated " + (DateTime.Now.Subtract(lastUpdatedDateTime)).TotalSeconds.ToString("F0") + "s ago \n";
    }

}
