// ********************************************* //
//							   					 //
//	  please write enum and const value class 	 //
//	  for this game project into this file.		 //
//												 //
// ********************************************* //

// const data class
public class MinoShape
{
	public enum Type : int
	{
		I_Mino = 0,
		O_Mino,
		T_Mino,
		J_Mino,
		L_Mino,
		S_Mino,
		Z_Mino,
		Ex1_Mino,
		Ex2_Mino,
		Ex3_Mino,
		Ex4_Mino,
		MinoNum	// please keep this element in the final
	};

	// **** attention! : rotating axis is index of 9 *** //
	static public readonly int AXIS_INDEX = 9;

	static public readonly int SHAPE_WIDTH = 4;
	static public readonly int SHAPE_HEIGHT = 4;

	// please write mino shape defining array into this class.
	// if you want to add a new shape, you must follow MinoShapeType list.

	public static readonly int[,] SHAPE =
	{
		// I_Mino shape
		{ 0, 1, 0, 0,
		  0, 1, 0, 0,
		  0, 1, 0, 0,
		  0, 1, 0, 0 },

		// O_Mino shape
		{ 0, 0, 0, 0,
		  0, 1, 1, 0,
		  0, 1, 1, 0,
		  0, 0, 0, 0 },

		// T_Mino shape
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  0, 1, 0, 0,
		  1, 1, 1, 0 },

		// J_Mino shape
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  1, 1, 1, 0,
		  0, 0, 1, 0 },

		// L_Mino shape
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  1, 1, 1, 0,
		  1, 0, 0, 0 },

		// S_Mino shape
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  1, 1, 0, 0,
		  0, 1, 1, 0 },

		// Z_Mino shape
		{ 0, 0, 0, 0, 
		  0, 0, 0, 0,
		  0, 1, 1, 0,
		  1, 1, 0, 0 },

		// Ex1_Mino
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  0, 1, 0, 0,
		  0, 0, 0, 0 },

		// Ex2_Mino
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  0, 1, 0, 0,
		  1, 0, 1, 0 },

		// Ex3_Mino
		{ 0, 0, 0, 0,
		  0, 0, 0, 0,
		  1, 1, 0, 0,
		  0, 0, 0, 0 },

		// Ex4_Mino
		{ 0, 0, 0, 0,
		  0, 1, 1, 0,
		  0, 1, 0, 0,
		  1, 1, 0, 0 }
	};
}

class ScoreValue
{
	static public readonly int STACK_SCORE = 100;
	static public readonly int ITEM_SCORE = 500;
}
