﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace WebAssembly
{
	/// <summary>
	/// Function bodies consist of a sequence of local variable declarations followed by bytecode instructions.
	/// </summary>
	public class FunctionBody
	{
		private IList<Local> locals;

		/// <summary>
		/// Local variables.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value cannot be set to null.</exception>
		public IList<Local> Locals
		{
			get => this.locals ?? (this.locals = new List<Local>());
			set => this.locals = value ?? throw new ArgumentNullException(nameof(value));
		}

		private IList<Instruction> code;

		/// <summary>
		/// Bytecode of the function.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value cannot be set to null.</exception>
		public IList<Instruction> Code
		{
			get => this.code ?? (this.code = new List<Instruction>());
			set => this.code = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Creates a new instance of <see cref="FunctionBody"/>.
		/// </summary>
		public FunctionBody()
		{
		}

		internal FunctionBody(Reader reader, long byteLength)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			var startingOffset = reader.Offset;
			var localCount = reader.ReadVarUInt32();
			var locals = this.Locals = new List<Local>(checked((int)localCount));
			for (var i = 0; i < localCount; i++)
				locals.Add(new Local(reader));

			this.code = Instruction.Parse(reader).ToArray();

			if (reader.Offset - startingOffset != byteLength)
				throw new ModuleLoadException($"Instruction sequence reader ended after readering {reader.Offset - startingOffset} characters, expected {byteLength}.", reader.Offset);
		}

		/// <summary>
		/// Expresses the value of this instance as a string.
		/// </summary>
		/// <returns>A string representation of this instance.</returns>
		public override string ToString() => $"Locals: {locals?.Count}, Code: {code?.Count}";
	}
}