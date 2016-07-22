using Pote.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a single program.
    /// </summary>
    internal sealed class Program : Node
    {
        private ObservableCollection<Procedure> procedures = new ObservableCollection<Procedure>();

        private ObservableCollection<String> strings = new ObservableCollection<String>();

        private ObservableCollection<Data> data = new ObservableCollection<Data>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program() : base(new InputPosition())
        {
            procedures.CollectionChanged += CollectionChanged;
            strings.CollectionChanged += CollectionChanged;
            data.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// All the procedures in the program.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The name of the procedure is not unique within the program.
        /// </exception>
        public IList<Procedure> Procedures
        {
            get
            {
                return procedures;
            }
        }

        /// <summary>
        /// All the strings in the program.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The name of the string is not unique within the program.
        /// </exception>
        public IList<String> Strings
        {
            get
            {
                return strings;
            }
        }

        /// <summary>
        /// All the data in the program.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The name of the data is not unique within the program.
        /// </exception>
        public IList<Data> Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// The origin of the program.
        /// </summary>
        public uint Origin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all the nodes in the program.
        /// </summary>
        private IEnumerable<INamedNode> AllNamedNodes
        {
            get
            {
                foreach (Procedure procedure in Procedures)
                    yield return procedure;

                foreach (String s in Strings)
                    yield return s;

                foreach (Data data in Data)
                    yield return data;
            }
        }

        /// <summary>
        /// Checks the names of add nodes for uniqueness.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The parameters for the event.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (INamedNode node in e.NewItems)
            {
                if (AllNamedNodes.Any(x => x.Name == node.Name))
                    throw new ArgumentException("The node name is not unique.");
            }
        }
    }
}