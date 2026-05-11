// FIXED VERSION (with Daily Report added)
using System;
using System.Collections.Generic;
using System.Linq;

// ENUM

enum SeverityLevel
{
    Normal = 2,
    Emergency = 1
}

// INTERFACE

interface IQueueable
{
    int GetPriority();
}

// BASE CLASS

abstract class Person
{
    private static int _counter = 1000;

    public string Id { get; }
    public string Name { get; }

    protected Person(string name)
    {
        Id = $"#{++_counter}";
        Name = name;
    }

    public abstract string GetRole();
}

// VISIT

class Visit
{
    public string Diagnosis { get; }

    public Visit(string diagnosis)
    {
        Diagnosis = diagnosis;
    }
}

// PATIENT

class Patient : Person, IQueueable
{
    public SeverityLevel Severity { get; }
    public string Condition { get; }
    public bool IsSeen { get; private set; }
    public bool IsCancelled { get; private set; }

    public Patient(string name, SeverityLevel severity, string condition) : base(name)
    {
        Severity = severity;
        Condition = condition;
    }

    public void UndoSeen()
    {
        IsSeen = false;
    }

    public override string GetRole() => "Patient";

    public int GetPriority() => (int)Severity;

    public void MarkAsSeen(string diagnosis)
    {
        IsSeen = true;
    }

    public void Cancel() => IsCancelled = true;
}

// DOCTOR

class Doctor : Person
{
    public bool IsAvailable { get; private set; } = true;

    public Doctor(string name) : base(name) { }

    public override string GetRole() => "Doctor";

    public void SetAvailability(bool val) => IsAvailable = val;
}

// CLINIC

class Clinic
{
    private readonly PriorityQueue<Patient, int> _queue = new();
    private readonly Dictionary<string, Patient> _patients = new();
    private readonly Stack<Action> _undo = new();

    private readonly List<Patient> _seenToday = new();
    private readonly DateTime _sessionStart = DateTime.Now;

    private Patient _current;

    public Patient Register(string name, SeverityLevel severity, string condition)
    {
        var p = new Patient(name, severity, condition);
        _patients[p.Id] = p;
        _queue.Enqueue(p, p.GetPriority());

        _undo.Push(() =>
        {
            p.Cancel();
            _patients.Remove(p.Id);
        });

        return p;
    }

    public Patient CallNext()
    {
        while (_queue.Count > 0)
        {
            var p = _queue.Dequeue();

            if (!p.IsSeen && !p.IsCancelled)
            {
                _current = p;

                _undo.Push(() =>
                {
                    _current = null;
                    _queue.Enqueue(p, p.GetPriority());
                });

                return p;
            }
        }

        throw new Exception("No patients");
    }

    public void Complete(string diagnosis)
    {
        if (_current == null) throw new Exception("No current patient");

        var temp = _current;
        temp.MarkAsSeen(diagnosis);
        _seenToday.Add(temp);
        _current = null;

        _undo.Push(() =>
        {
            temp.UndoSeen();
            _seenToday.Remove(temp);
            _queue.Enqueue(temp, temp.GetPriority());
        });
    }

    public string Undo()
    {
        if (_undo.Count == 0) return "Nothing";
        _undo.Pop().Invoke();
        return "Undone";
    }

    public int GetWaitingCount()
    {
        return _patients.Values.Count(p => !p.IsSeen && !p.IsCancelled);
    }

    public void PrintReport()
    {
        var duration = DateTime.Now - _sessionStart;

        Console.WriteLine("\n==============================");
        Console.WriteLine("      Daily Report");
        Console.WriteLine("==============================");

        Console.WriteLine($"Total Registered : {_patients.Count}");
        Console.WriteLine($"Seen Today       : {_seenToday.Count}");
        Console.WriteLine($"Waiting          : {GetWaitingCount()}");
        Console.WriteLine($"Session Time     : {duration.Hours}h {duration.Minutes}m");

        Console.WriteLine("==============================\n");
    }
}

// MAIN

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var clinic = new Clinic();

        while (true)
        {
            Console.WriteLine("1 Register\n2 Call\n3 Complete\n4 Undo\n5 Report\n0 Exit");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Name: ");
                        var name = Console.ReadLine();
                        Console.Write("Condition: ");
                        var cond = Console.ReadLine();
                        Console.Write("1 Normal 2 Emergency: ");
                        var s = Console.ReadLine() == "2" ? SeverityLevel.Emergency : SeverityLevel.Normal;

                        var p = clinic.Register(name, s, cond);
                        Console.WriteLine($"Added {p.Id}");
                        break;

                    case "2":
                        var next = clinic.CallNext();
                        Console.WriteLine($"Calling {next.Name}");
                        break;

                    case "3":
                        Console.Write("Diagnosis: ");
                        clinic.Complete(Console.ReadLine());
                        break;

                    case "4":
                        Console.WriteLine(clinic.Undo());
                        break;

                    case "5":
                        clinic.PrintReport();
                        break;

                    case "0": return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}