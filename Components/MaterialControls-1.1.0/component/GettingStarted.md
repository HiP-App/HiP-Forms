# Getting Started

For some controls (Button, Table View Cell, Text Field), developer only needs to change from standard iOS control, such as UIButton, to material design control, MDButton, and that works as a charm.

For other controls (Switch, Slider, Progress, Date Picker, Tab Bar) which inherits from UIView, developer needs to:

- Drag UIView to designer 
- Change class from UIView to relevant material control class, such as MDSwitch, MDSlider

For Time Picker, it should be displayed in dialog, so developer should initiate it in code like below:

```
var dialog = new MDTimePickerDialog();
dialog.Show();
```