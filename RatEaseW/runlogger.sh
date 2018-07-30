#run=0;
#echo " run is: $run"
# Make sure to save this without carraig returns

while :
	do
			
			
			
			if [ -f newset ]
			then
			echo "newset file exists"
			rm newset
			
				rm *.txt;
				
			  files = ls *.bmp;
				echo "$files"
				for filename in ls *.bmp; do
				  
				  let idx++;
				  
				  tesseract $filename t$idx -l eng -oem 3 -psm 7
				  rm $filename
			done
			fi
			
			#pause a second.
			echo "sleep1"
			sleep 1;
			
	done
echo "stopped"
