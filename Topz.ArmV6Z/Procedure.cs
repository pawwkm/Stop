﻿using Pote.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A single procedure in a program.
    /// </summary>
    internal sealed class Procedure : Node, INamedNode
    {
        private ObservableCollection<Instruction> instructions = new ObservableCollection<Instruction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Procedure"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="name">Name of the procedure.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Procedure(InputPosition position, string name) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            instructions.CollectionChanged += CheckLabelUniqueness;
        }

        /// <summary>
        /// Checks that any added instruction's label is unique within the procedure.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The parameters for the event.</param>
        private void CheckLabelUniqueness(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            var labledInstructions = (from i in Instructions
                                      where i.Label != null
                                      select i).ToArray();

            foreach (Instruction instruction in e.NewItems)
            {
                if (labledInstructions.Any(x => x.Label.Name == instruction.Label.Name))
                    throw new ArgumentException(instruction.Position.ToString($"Redefining the label '{instruction.Label.Name}'."));
            }
        }

        /// <summary>
        /// Name of the procedure.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the this is the main procedure of the program.
        /// </summary>
        public bool IsMain
        {
            get;
            set;
        }

        /// <summary>
        /// The instructions that the procedure is composed of in 
        /// the order they are to be executed.
        /// </summary>
        public IList<Instruction> Instructions
        {
            get
            {
                return instructions;
            }
        }
    }
}