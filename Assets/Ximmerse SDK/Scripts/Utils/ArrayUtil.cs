
public class ArrayUtil {

	/// <summary>
	/// 
	/// </summary>
	public static void MemCpy<T>(T[] destArray,int destOffset,int destDir,T[] srcArray,int srcOffset,int srcDir,int count) {
		while(count-->0) {
			destArray[destOffset]=srcArray[srcOffset];
			destOffset+=destDir;
			srcOffset+=srcDir;
		}
	}
}
