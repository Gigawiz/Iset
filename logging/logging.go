package logging

import (
	"fmt"
)

var LogToConsole = true
func Log(logLine string) {
	if (LogToConsole) {
		fmt.Println(logLine)
	}
}