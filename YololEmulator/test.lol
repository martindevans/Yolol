if :lock == 0 then			// Check if lock is free
	:lock = 185				// Take lock (set it to your unique ID)
	if :lock == 185 then	// Check if you managed to take the lock
		goto protected		// You did, go to protected section
	end
end
goto 0						// Didn't get the lock, go back to try again

protected:					// Code executing here holds the lock

:lock = 0					// Release the lock
