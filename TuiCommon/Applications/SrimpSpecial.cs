namespace TuiCommon.Applications;

// all text generated with https://www.asciiart.eu/text-to-ascii-art
// meme based on https://youtu.be/Go6DRgNaCL8
public class SrimpSpecial(ScreenBase screenBase) : TuiApplication(screenBase) {
    protected internal override void Start() {
        /*Sb.SetDirty();*/
    }

    private int _animationTicks = 0;
    private byte _color = 46;
    private byte? _bgColor = null;

    private int cutoff1 = 0;
    private int cutoff2;
    private int cutoff3;
    private int cutoff4;
    
    protected internal override void Tick() {
        _animationTicks++;
        cutoff1 = _animationTicks / 2;
        cutoff2 = cutoff1 - 60;
        cutoff3 = cutoff2 - 60;
        cutoff4 = cutoff3 - 60;

        int flashCutoff = _animationTicks/16 - 183/8;
        if (0 >= flashCutoff) return;
        _color = 226;
        _bgColor = flashColors[Math.Clamp(flashCutoff, 0, flashColors.Length - 1)];
    }

    protected internal override void Render() {
        Sb.SetForegroundColor(0,0, _color);
        if (_bgColor != null) Sb.SetBackgroundColor(0,0,_bgColor.Value);
        
        // absolutely position the text, the text's dimentions are 104 width and 26 height, divide by 2 to get center.
        Sb.DrawString((Sb.Center.x - 52, Sb.Center.y - 13), ascii[0].TrimLines(cutoff2, cutoff1));
        Sb.DrawString((Sb.Center.x - 52, Sb.Center.y - 13), ascii[1].TrimLines(cutoff3, cutoff2));
        Sb.DrawString((Sb.Center.x - 52, Sb.Center.y - 13), ascii[2].TrimLines(cutoff4, cutoff3));
        Sb.DrawString((Sb.Center.x - 52 + Math.Clamp(- cutoff4*2 + 160, 0, 1000), Sb.Center.y - 13), ascii[3]); // srimp
    }
    
    
    public static string[] ascii = [
        """
                                        ######  ######   ####   ##  ##   ####                                   
                                            ##  ##      ##      ##  ##  ##                                      
                                            ##  ####     ####   ##  ##   ####                                   
                                        ##  ##  ##          ##  ##  ##      ##                                  
                                         ####   ######   ####    ####    ####                                   
                                                                                                                
                                                                                                                
             ####   ##  ##   ####   ##   ##  ######  #####   ######  #####                   ##  ##  ######     
            ##  ##  ### ##  ##      ##   ##  ##      ##  ##  ##      ##  ##                  ##  ##    ##       
            ######  ## ###   ####   ## # ##  ####    #####   ####    ##  ##    ##             #  #     ##       
            ##  ##  ##  ##      ##  #######  ##      ##  ##  ##      ##  ##    ##                      ##       
            ##  ##  ##  ##   ####    ## ##   ######  ##  ##  ######  #####      #                    ######     
                                                                                                                
                                                                                                                
                    ####   ##   ##          ######  ##  ##  ######          ##   ##   ####   ##  ##             
                   ##  ##  ### ###            ##    ##  ##  ##              ##   ##  ##  ##   ####              
                   ######  ## # ##            ##    ######  ####            ## # ##  ######    ##               
                   ##  ##  ##   ##            ##    ##  ##  ##              #######  ##  ##    ##               
                   ##  ##  ##   ##            ##    ##  ##  ######           ## ##   ##  ##    ##               
                                                                                                                
                                                                                                                
          ####   ##  ##  #####           ######  ##  ##  ######          ######  #####   ##  ##  ######  ##  ## 
         ##  ##  ### ##  ##  ##            ##    ##  ##  ##                ##    ##  ##  ##  ##    ##    ##  ## 
         ######  ## ###  ##  ##            ##    ######  ####              ##    #####   ##  ##    ##    ###### 
         ##  ##  ##  ##  ##  ##            ##    ##  ##  ##                ##    ##  ##  ##  ##    ##    ##  ## 
         ##  ##  ##  ##  #####             ##    ##  ##  ######            ##    ##  ##   ####     ##    ##  ## 
        """,
        
        
        
        
        
        """
          ####   ##  ##  #####           ######  ##  ##  ######          ##      ######  ######  ######         
         ##  ##  ### ##  ##  ##            ##    ##  ##  ##              ##        ##    ##      ##             
         ######  ## ###  ##  ##            ##    ######  ####            ##        ##    ####    ####           
         ##  ##  ##  ##  ##  ##            ##    ##  ##  ##              ##        ##    ##      ##        ##   
         ##  ##  ##  ##  #####             ##    ##  ##  ######          ######  ######  ##      ######    ##   
                                                                                                                
             ##  ##   ####            ####   ##  ##  ######           ####    ####   ##   ##  ######   ####     
             ### ##  ##  ##          ##  ##  ### ##  ##              ##  ##  ##  ##  ### ###  ##      ##        
             ## ###  ##  ##          ##  ##  ## ###  ####            ##      ##  ##  ## # ##  ####     ####     
             ##  ##  ##  ##          ##  ##  ##  ##  ##              ##  ##  ##  ##  ##   ##  ##          ##    
             ##  ##   ####            ####   ##  ##  ######           ####    ####   ##   ##  ######   ####     
                                                                                                                
         ######   ####           ######  ##  ##  ######          ######   ####   ######  ##  ##  ######  #####  
           ##    ##  ##            ##    ##  ##  ##              ##      ##  ##    ##    ##  ##  ##      ##  ## 
           ##    ##  ##            ##    ######  ####            ####    ######    ##    ######  ####    #####  
           ##    ##  ##            ##    ##  ##  ##              ##      ##  ##    ##    ##  ##  ##      ##  ## 
           ##     ####             ##    ##  ##  ######          ##      ##  ##    ##    ##  ##  ######  ##  ## 
                                                                                                                
                                     ######  ##  ##   ####   ######  #####   ######                             
                                     ##       ####   ##  ##  ##      ##  ##    ##                               
                                     ####      ##    ##      ####    #####     ##                               
                                     ##       ####   ##  ##  ##      ##        ##                               
                                     ######  ##  ##   ####   ######  ##        ##                               
                                                                                                                
        """,
        
        """









                      ######  ##  ##  #####    ####   ##  ##   ####   ##  ##          ##   ##  ###### 
                        ##    ##  ##  ##  ##  ##  ##  ##  ##  ##      ##  ##          ### ###  ##     
                        ##    ######  #####   ##  ##  ##  ##  ## ###  ######          ## # ##  ####   
                        ##    ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##          ##   ##  ##     
                        ##    ##  ##  ##  ##   ####    ####    ####   ##  ##          ##   ##  ###### 









                                                                                                                
        """,
        
        
        
        
        """
        
        
        
                                                      ##     #####                                         
                                                     ##     ## #                                           
                                                    #####    ####                                          
                                                    ##  ##    # ##                                         
                                                     ####   #####                                          
                                                                                        
                                                                                        
                                                                                        
                                          ####   #####   ######  ##   ##  #####                 
                                         ##      ##  ##    ##    ### ###  ##  ##                
                                          ####   #####     ##    ## # ##  #####                 
                                             ##  ##  ##    ##    ##   ##  ##                    
                                          ####   ##  ##  ######  ##   ##  ##                    
                                                                                        
                                                                                        
                                                                                        
                                  ####   #####   ######   ####   ######   ####   ##     
                                 ##      ##  ##  ##      ##  ##    ##    ##  ##  ##     
                                  ####   #####   ####    ##        ##    ######  ##     
                                     ##  ##      ##      ##  ##    ##    ##  ##  ##     
                                  ####   ##      ######   ####   ######  ##  ##  ###### 
                                                                
        
                                                                                                                
        """  
    ];

    private byte[] flashColors = [
        226, 220, 214, 208, 202, 166, 130, 94, 52, 0
    ];
}