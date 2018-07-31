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
			
			if [ -f newset ]
			then
			
			
			echo "newset file exists"
			rm newset
			
				rm *.txt;
				
			  files = ls *.bmp;
				echo "$files"
				for filename in ls t*.bmp; do
				  
				  let idx++;
				  echo "$filename  t$idx"
				  tesseract $filename t$idx -l eng -psm 7
				  rm $filename
			done
			fi
			
			#pause a second.
			if [ ! -f system.txt ]
			then
				echo "system.bmp -> system.txt"
				tesseract system.bmp system -l eng -psm 7
			fi
			echo "sleep1"
			sleep 1;
			
	done
echo "stopped"