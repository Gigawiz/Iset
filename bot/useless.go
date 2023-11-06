// snippet from: bot/useless.go
package bot

import (
	"strconv"
	"strings"
)

func Unflip() string {
	lines := []string{
		`\u252C`,
		`\u2500`,
		`\u252C`,
		`\uFEFF`,
		`\u0020`,
		`\u30CE`,
		`\u0028`,
		`\u0020`,
		`\u309C`,
		`\u002D`,
		`\u309C`,
		`\u30CE`,
		`\u0029`,
	}
	for i, v := range lines {
		lines[i], _ = strconv.Unquote(`"` + v + `"`)
	}
	return strings.Join(lines, "")
}