namespace Traceroute;

public class MyICMP
{
    public MyICMP(byte[] package)
    {
        package[0] = 8;  //msg type
        package[1] = 0;  //msg code
        
        package[2] = 0;  //checksum
        package[3] = 0;
        
        package[4] = 0;  //identifier
        package[5] = 1;
        
        package[6] = 0;  //sequence number
        package[7] = 1;
    }
    
    public void sequenceNumber(byte[] package, int number)
    {
        package[6] = (byte)(number >> 8);
        package[7] = (byte)(number);
    }
    
    public void checkSum(byte[] package)
    {
        uint checkSum = ((uint)package[0] << 8) + ((uint)package[1]);
        uint tmp = 0;
        for (int i = 4; i < package.Length; i += 2)
        {
            tmp = (uint)(package[i] << 8);
            tmp += (uint)package[i + 1];
            checkSum += tmp;
        }
        checkSum = (uint)(~checkSum);
        package[2] = (byte)(checkSum >> 8);
        package[3] = (byte)(checkSum);
    }
    
}