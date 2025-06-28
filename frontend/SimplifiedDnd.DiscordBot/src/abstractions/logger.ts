export enum Style {
  Rest = '\x1b[0m',
  Bright = '\x1b[1m',
  Dim = '\x1b[2m',
  Underscore = '\x1b[4m',
  Blink = '\x1b[5m',
  Reverse = '\x1b[7m',
  Hidden = '\x1b[8m',
}

export enum Color {
  Black = '\x1b[30m',
  Red = '\x1b[31m',
  Green = '\x1b[32m',
  Yellow = '\x1b[33m',
  Blue = '\x1b[34m',
  Magenta = '\x1b[35m',
  Cyan = '\x1b[36m',
  White = '\x1b[37m',
}

export default class Logger {
  readonly dividerCharacter: string = '-';

  logInfo(data: string): void {
    console.log(`${this.toColor(Color.Cyan, 'info')}:\n  ${data}`);
  }

  logSuccess(data: string): void {
    console.log(`${this.toColor(Color.Green, 'success')}:\n  ${data}`);
  }

  logWarning(data: string): void {
    console.log(`${this.toColor(Color.Yellow, 'warning')}:\n  ${data}`);
  }

  logError(data: string): void {
    console.log(`${this.toColor(Color.Red, 'error')}:\n  ${data}`);
  }

  logDivider(data: string): void {
    const DIVIDER_SIZE = 30;
    const paddingAmount = DIVIDER_SIZE - data.length;
    let divider = this.toStyle(Style.Bright, data);
    if (paddingAmount > 0) {
      const padding = this.dividerCharacter.repeat(paddingAmount / 2 - 1);
      divider = `${padding} ${divider} ${padding}`;
    }
    console.log(`\n${divider}`);
  }

  private toColor(color: Color, data: string): string {
    return `${color}${data}${Style.Rest}`;
  }

  private toStyle(style: Style, data: string): string {
    return `${style}${data}${Style.Rest}`;
  }
}
