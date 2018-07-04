# Nanoservice

[UConn Computational and Systems Lab](https://csml.uconn.edu)

[Hyponatremia Decision Support Demo](https://github.com/yshin1209/Nanoservice/blob/master/NanoServiceAPI%20Demo.pptx) (MATLAB version is also available [here](https://github.com/yshin1209/Nanoservice/blob/master/NanoserviceAPI_Demo_MATLAB.m))

Modern distributed systems and networks (e.g., smart and connected health, smart grid, sensor networks, etc.) should perform real-time learning, decision-making, and control. Often the properties (other names: variables, fields) and actions (other names: methods, functions) of each system or network component cannot be clearly defined in advance. In addition, the network topology among these components may dynamically change over time. Therefore, hard-coding the properties, actions, and neighbors of every network component becomes cumbersome in the presence of such uncertainty as they need to be added, updated, or removed under dynamically-changing circumstances. For example, a patient model with “body temperature” property may need to add “blood glucose level” property when the continuous glucose monitoring (CGM) data become available from a wearable CGM device. Since a human body can have virtually an unlimited number of properties, it is not possible or even necessary to hard-code them in advance. For the same reason, actions can be decoupled and outsourced without being hard-coded. In this case, each network component consumes external action services which provide 1) knowledge via relational/logical operations (e.g., hyperglycemia is diagnosed when the blood glucose level is greater than 200 mg/dL) for high-level decision making and 2) engineering algorithms such as adaptive system identification and model predictive control for physical model-based tasks (e.g., controlling blood glucose levels so that they become below 200 mg/dL). Two example action services (PID control as a Service and LMS adaptive parameter estimation as a Service) can be found at https://github.com/uconn-csml/EaaS.

[Click this link and download the simple demo pdf!](https://github.com/yshin1209/Nanoservice/blob/master/NanoServiceAPI%20Demo.pdf)


