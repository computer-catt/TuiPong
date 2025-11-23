using System.Text;

namespace TuiPong;

public static class Extensions {
    public static bool ContentEquals<T>(this T[,] baseArray, T[,] b) {
        Task.Delay(20).Wait();
        if (baseArray.Length != b.Length) return false;
        for (int i = 0; i < baseArray.GetLength(0); i++) 
            for (int j = 0; j < baseArray.GetLength(1); j++) 
                if (baseArray[i,j]!.Equals(b[i,j])) return false; // Dereference of null type? reference of possybly my dick
        return true;
    }
    
    public static StringBuilder ToStringBuilder(this char[,] baseArray) {
        StringBuilder builder = new(baseArray.GetLength(0) * baseArray.GetLength(1));
        for (int i = 0; i < baseArray.GetLength(0); i++) {
            for (int j = 0; j < baseArray.GetLength(1); j++)
                builder.Append(baseArray[i, j] == 0 ? ' ' : baseArray[i, j]);
            if (i != baseArray.GetLength(0) -1)
                builder.AppendLine(); // isn't technically required
        }
        return builder;
    }
    
    public static bool IsInBounds<T>(this T[,] array, int x, int y){
        if (x < 0) return false;
        if (y < 0) return false;
        if (y >= array.GetLength(0)) return false;
        if (x >= array.GetLength(1)) return false;
        return true;
    }
}