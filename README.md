# Logic Circuit Simulator

## Overview
This project is a powerful logic circuit simulator built using WPF in C#. The application allows users to create, manipulate, and simulate digital logic circuits. Users can design circuits by placing logic gates, connecting them with wires, and observing the flow of signals through the circuit. The simulator offers advanced features for circuit creation, signal visualization, and interaction, making it a valuable tool for both learning and experimentation in digital logic design.

## Key Features
### Dynamic Logic Gates
Resizable Input Ports: Logic gates can be resized dynamically to add or remove input ports. Adding input ports is done by stretching the gate, and only empty ports are removed when shrinking. This feature provides flexibility in circuit design and is a unique feature not commonly found in other simulators.

### Infinite Loop Handling: 
The simulator includes mechanisms to detect and handle infinite loops, which can occur in complex logic circuits. A slight delay is introduced in the signal propagation to avoid bottlenecks and ensure that the application remains responsive even when loops are present. This prevents the simulator from crashing or freezing when dealing with circuits that involve feedback loops.

### Custom Creation Menu
#### Nested Folders and Items
The creation menu allows users to create logic components through a nested folder structure. The menu supports both direct and inverted logic gates, output components (like toggles and constants), and input components (like pixels and hex displays). The menu is context-sensitive and can be opened with a right-click on the main grid, positioning itself near the cursor for easy access.
#### Persistence of Menu State 
The state of the creation menu, including which folders are open or closed, is cached between uses. This ensures that the user's preferences are maintained, making the design process smoother and more intuitive.

### Signal Visualization
#### Real-Time Signal Display 
Wires and logic gates visually update in real-time to reflect the current state of signals passing through them. Wires change color based on the signal they carry (e.g., active, inactive, or error states), providing immediate feedback on the circuit's operation.
#### Dynamic Wire Drawing
Wires are drawn using Bezier curves, which dynamically adjust based on the positions of connected ports. This allows for clean and visually appealing circuit diagrams.

### Infinite Loop Prevention
To prevent infinite loops in circuits, the simulator introduces a small delay during signal propagation. This delay ensures that signals stabilize before being re-evaluated, preventing the system from getting stuck in an infinite loop. This feature is particularly important for circuits that involve feedback or latch mechanisms.

## Installation
To run this project, ensure you have .NET installed. Clone the repository and build the solution in your preferred IDE.

```
git clone https://github.com/Austin-bryan/LogicLab
```

## Usage
- **Creating Circuits**: Right-click on the main grid to open the creation menu. Select components from the nested folders to add them to your circuit.
- **Connecting Components**: Use the drag-and-drop interface to connect logic gates with wires. The simulator will automatically update the signal flow as you modify the circuit.
- **Resizing Gates**: Click and drag the edges of a logic gate to add or remove input ports dynamically.
- **Simulating Circuits**: Observe how signals propagate through the circuit in real-time. The wires will change color based on the signals they carry.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue if you have ideas for new features or find bugs.

## Authors
This project was developed by Austin Bryan and other contributors as part of an application development course.

