# This is an example file of a test for my assembler, vm and CPU
# If it jumps(calli) to FAIL then the test has failed. If it jumps(calli) to DONE the the test passed

DONE:
	br DONE
	
FAIL:
	br FAIL
	
test_1:
	set s0, 42
	set s1, 2
	
	calli int_mul
	
	#Expected = 42 * 2
	set s2, 84
	
	sub t0, t0, s2
	bnez t0, FAIL
	
	br DONE
	
#-----------------------------------------------------------------------
# Multiplies 2 s0 and s1 and stores the result in t0. All registers are
# treated as unsigned integers.
# NB: this is sub-optimal.
#-----------------------------------------------------------------------	
int_mul:
		add t0, zero, zero #t0 = 0
		add t1, zero, s0 #t1 = s0
	
	int_mul_next:
		beqz t1, int_mul_end # if(t1 != 0){
			add t0, t0, s1 	   # 	t0 += s1
			sub t1, t1, one	   #	t1 -= 1
	
			br int_mul_next	# }
		
	int_mul_end:
		ret
