using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PokemonGroveGreen
{
    [Serializable]
    public class MyList<T> : IEnumerable<T>
    {
        //Author: Roberto Ortiz Perez
        Node<T> first;
        int length;

        public MyList()
        {
            first = null;
            length = 0;
        }

        public bool Add(T element)
        {
            // Add a new element
            Node<T> newNode = new Node<T>(element);

            if (first == null)
                first = newNode;
            else
            {
                Node<T> current = first;
                while (current.GetNext() != null)
                {
                    current = current.GetNext();
                }
                current.SetNext(newNode);
            }

            length += 1;
            return true;
        }

        public bool Remove(T element)
        {
            Node<T> current = first;
            Node<T> previous = null;

            while (current != null)
            {
                if (current.GetElement().Equals(element))
                {
                    if (previous == null)
                        first = current.GetNext();
                    else
                        previous.SetNext(current.GetNext());

                    length -= 1;
                    return true;
                }

                previous = current;
                current = current.GetNext();
            }

            return false;
        }

        public T GetElem(int index)
        {
            try
            {
                if (index < 0 || index >= length)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }

                Node<T> current = first;
                for (int i = 0; i < index; i++)
                {
                    current = current.GetNext();
                }
                return current.GetElement();
            }
            catch (IndexOutOfRangeException)
            {
                return default(T);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class MyListEnumerator : IEnumerator<T>
        {
            private MyList<T> _list;
            private Node<T> _current;
            private Node<T> _previous;

            public MyListEnumerator(MyList<T> list)
            {
                _list = list;
                _current = null;
                _previous = null;
            }

            public T Current => _current.GetElement();

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_current == null)
                {
                    _current = _list.first;
                }
                else
                {
                    _previous = _current;
                    _current = _current.GetNext();
                }

                return _current != null;
            }

            public void Reset()
            {
                _current = null;
                _previous = null;
            }

            public void Dispose() { }
        }
    }

    [Serializable]
    public class Node<T>
    {
        T element;
        Node<T> next;

        public Node(T element)
        {
            this.element = element;
            this.next = null;
        }

        public Node(T element, Node<T> next)
        {
            this.element = element;
            this.next = next;
        }

        public T GetElement() { return element; }
        public Node<T> GetNext() { return next; }
        public void SetElement(T element) { this.element = element; }
        public void SetNext(Node<T> next) { this.next = next; }
    }

    class CarDealership
    {
        //public Vehicle Search(string licensePlate)
        //{
        //    //Search for a vehicle in our dealership with the same license plate.
        //    //Returns the vehicle if we find it, 'null' if we do not.
        //
        //    //If we do not have a vehicle stored a new exception is thrown.
        //    if (first == null)
        //        throw new EmptyDealership();
        //
        //    Node current = first;
        //
        //    //We keep searching meanwhile the dealership has vehicles.
        //    while (current != null)
        //    {
        //        if (current.GetVehicle().Equals(licensePlate))
        //            return current.GetVehicle(); //We do have the vehicle
        //        current = current.GetNext();
        //    }
        //    return null; //We do not have this vehicle
        //}
        
        //public bool Add(Vehicle newVehicle)
        //{
        //    //Add a new vehicle unless we already have one with the same plate.
        //    //Returns 'true' if we succeed, 'false' if we do not.
        //
        //    //Clause activates in case the users enters an already existing license plate.
        //    if (newVehicle == null)
        //        return false;
        //
        //    string newPlate = newVehicle.GetLicensePlate();
        //    Node newNode = new Node(newVehicle);
        //
        //    //The dealership is empty, so we store the vehicle on the first position.
        //    if (first == null)
        //        first = newNode;
        //
        //    else
        //    {
        //        Node current = first;
        //        Node previous = null;
        //
        //        //We cycle through the dealership until the end.
        //        while (current.GetNext() != null)
        //        {
        //            //If this is true the new vehicle should be stored before the current one.
        //            if (!current.GetVehicle().IsLower(newPlate))
        //            {
        //                //Store the vehicle.
        //                previous.SetNext(newNode);
        //                newNode.SetNext(current);
        //                length += 1;
        //                return true;
        //            }
        //            //Cycle through the list.
        //            previous = current;
        //            current = current.GetNext();
        //        }
        //
        //        //We need to compare the last element of the list.
        //        if (current.GetVehicle().IsLower(newPlate))
        //            current.SetNext(newNode);
        //        else
        //        {
        //            if (previous == null) //In case we only have 1 element on the list (we would not be able to use prev here).
        //            {
        //                //Add the vehicle before this only element.
        //                first = newNode;
        //                newNode.SetNext(current);
        //            }
        //            else
        //            {
        //                //Add the vehicle before the last element.
        //                previous.SetNext(newNode);
        //                newNode.SetNext(current);
        //            }
        //        }
        //    }
        //    length += 1;
        //    return true;
        //}
        
        //public override string ToString()
        //{
        //    //Returns the entirety of the dealership information in a proper format. Calls Node ToString.
        //    if (first == null)
        //        return "The dealership is empty.";
        //    else
        //    {
        //        string aux = "";
        //
        //        Node current = first;
        //
        //        //We cycle through the dealership until the end.
        //        do
        //        {
        //            aux += current.ToString();
        //            current = current.GetNext();
        //
        //        } while (current != null);
        //
        //        return aux;
        //    }
        //}
        
        //public bool UpdateMileage(string licensePlate, int addition)
        //{
        //    //Add mileage to an existing vehicle. Return true if we succeed, false in any other case
        //
        //    //If we do not have a vehicle stored a new exception is thrown.
        //    if (first == null)
        //        throw new EmptyDealership();
        //
        //    Vehicle aux = Search(licensePlate);
        //
        //    if (aux == null) //We have not found the vehicle.
        //        return false;
        //    else
        //    {
        //        //We add the mileage to the car associated to the given license plate.
        //        aux.AddMileage(addition);
        //        return true;
        //    }
        //}
        
        //public bool Delete(string licensePlate)
        //{
        //    //Delete a vehicle from the dealership if we have one with said plate. Returns 'true if we succeed.'
        //
        //    //If we do not have a vehicle stored a new exception is thrown.
        //    if (first == null)
        //        throw new EmptyDealership();
        //
        //    Node current = first.GetNext();
        //    Node previous = first;
        //
        //    if (current != null) //We have more than one element on the list
        //    {
        //        //We cycle through the dealership until the end.
        //        do
        //        {
        //            if (current.GetVehicle().Equals(licensePlate)) //Compare the vehicle.
        //            {
        //                //If we have a match we delete the current vehicle.
        //                previous.SetNext(current.GetNext());
        //                length -= 1;
        //                return true;
        //            }
        //            else //Cycle through the dealership.
        //            {
        //                previous = current;
        //            }
        //            current = current.GetNext();
        //        } while (current != null);
        //    }
        //
        //    //It is the only element on the list
        //    if (first.GetVehicle().Equals(licensePlate))
        //    {
        //        first = first.GetNext();
        //        length -= 1;
        //        return true;
        //    }
        //
        //    return false;
        //}
    }

    class Node
    {
        //Author: Roberto Ortiz Pérez.
        //Class used for the proper functioning of the 'CarDealership' class.

        //Vehicle vehicle;
        //Node next; //Used to make a list.

        //public override string ToString()
        //{
        //    //Calls and returns the method 'ToString' of the vehicle stored in this node.
        //    return vehicle.ToString() + "\n";
        //}
    }
}
