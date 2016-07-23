using Pote.Text;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The add with carry instruction.
    /// </summary>
    /// <remarks>See A.4.1.2 for more info.</remarks>
    internal class AddWithCarryInstruction : Instruction
    {
        public AddWithCarryInstruction(Label label, Mnemonic mnemonic) : base(label, mnemonic)
        {
        }

        public RegisterOperand Desitnation
        {
            get;
            set;
        }

        public RegisterOperand FirstOperand
        {
            get;
            set;
        }

        public ShifterOperand ShifterOperand
        {
            get;
            set;
        }
    }
}