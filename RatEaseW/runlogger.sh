#run=0;
#echo " run is: $run"
# Make sure to save this without carraig returns

#run=0;
#echo " run is: $run"
declare -i sys;
declare -i idx;
#let sys =: 0;


while :
	do
			
#		if (sys > 5);
#			then
#				let sys = 0;
#				echo "gt 5  tesserct system"
#				tesseract system.bmp system -l eng -oem 3 
#			fi
#			let sys++;
			
			if [ -f newset ]
			then
			
			
			echo "newset file exists"
			rm newset
			
				rm *.txt;
				
			  files = ls *.bmp;
				echo "$files"
				for filename in ls t*.bmp; do
				  
				  let idx++;
				  
				  tesseract $filename t$idx -l eng -oem 3 
				  rm $filename
			done
			fi
			
			#pause a second.
			if [ ! -f system.txt ]
			then
				tesseract system.bmp system -l eng -oem 3 
			fi
			echo "sleep1"
			sleep 1;
			
	done
echo "stopped"

