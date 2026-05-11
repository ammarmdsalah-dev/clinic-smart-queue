# Clinic Smart Queue

A C# console application that manages a clinic waiting room queue.
Emergency patients are automatically prioritized over normal cases, the receptionist can undo any mistake, and a daily report is generated at the end of the session.

---

## The Problem

In most clinics, patients wait in a single line regardless of how serious their condition is.
A patient with chest pain waits behind someone who came for a routine checkup.
This system fixes that by automatically putting emergency cases first.

---

## Features

| Feature | Description |
|---|---|
| Priority Queue | Emergency patients are always called before normal ones |
| Undo System | Reverse the last action at any point |
| Daily Report | End-of-session summary of registered and seen patients |

---

## How to Run

**Requirements:** .NET 6 or later

```bash
git clone https://github.com/your-username/clinic-smart-queue.git
cd clinic-smart-queue
dotnet run
```

Or open the `.csproj` file in Visual Studio and press `Ctrl + F5`.

---

## Menu Options

```
1 - Register new patient
2 - Call next patient
3 - Complete current visit
4 - Undo last action
5 - Daily report
0 - Exit
```

---

## OOP Concepts Used

| Concept | Where |
|---|---|
| Abstraction | `Person` is abstract — cannot be instantiated directly |
| Encapsulation | Patient fields are private, accessed only through methods |
| Inheritance | `Patient` and `Doctor` both inherit from `Person` |
| Polymorphism | `GetRole()` is overridden differently in each subclass |
| Interface | `IQueueable` enforces `GetPriority()` on any queued object |

---

## Data Structures Used

| Structure | Purpose |
|---|---|
| `PriorityQueue<Patient, int>` | Emergency patients (priority 1) come before normal ones (priority 2) |
| `Dictionary<string, Patient>` | Instant O(1) patient lookup by ID |
| `Stack<Action>` | Stores undo actions in LIFO order |
| `List<Patient>` | Tracks patients seen today for the daily report |



---

## License

MIT — free to use, modify, and share.
