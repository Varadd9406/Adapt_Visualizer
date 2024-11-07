using UnityEngine;
using UnityEngine.UIElements;

public class VideoSettingsController : MonoBehaviour
{
    private UIDocument document;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        // Set up tab buttons
        var basicTab = root.Q<Button>("basic-tab");
        var advancedTab = root.Q<Button>("advanced-tab");

        basicTab.clicked += () => SetActiveTab();
        advancedTab.clicked += () => SetActiveTab();

        // Set up toggle buttons
        SetupToggleButton(root, "ads-field-of-view-toggle");
        SetupToggleButton(root, "ads-dof-effects-toggle");
        SetupToggleButton(root, "chromatic-aberration-toggle");
        SetupToggleButton(root, "film-grain-toggle");
        SetupToggleButton(root, "vignette-toggle");
        SetupToggleButton(root, "lens-distortion-toggle");

        // Set up sliders
        SetupSlider(root, "brightness-slider", 0, 100, 55);
        SetupSlider(root, "field-of-view-slider", 0, 100, 74);
        SetupSlider(root, "vehicle-fov-slider", 0, 100, 95);
        SetupSlider(root, "motion-blur-slider", 0, 100, 0);
    }

    private void SetupToggleButton(VisualElement root, string name)
    {
        var toggle = root.Q<Button>(name);
        var isOn = false;

        toggle.clicked += () =>
        {
            isOn = !isOn;
            toggle.text = isOn ? "ON" : "OFF";
            toggle.ToggleInClassList("toggle-on");
        };
    }

    private void SetupSlider(VisualElement root, string name, float min, float max, float defaultValue)
    {
        var slider = root.Q<Slider>(name);
        slider.lowValue = min;
        slider.highValue = max;
        slider.value = defaultValue;
    }

    private void SetActiveTab()
    {
        var basicTab = document.rootVisualElement.Q("basic-tab");
        var advancedTab = document.rootVisualElement.Q("advanced-tab");

        basicTab.ToggleInClassList("active-tab");
        advancedTab.ToggleInClassList("active-tab");
    }
}