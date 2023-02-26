export const environment = {
  production: true,
  backendUrl: 'https://localhost:7157/api',
  errorMessages: {
    passwordFormat: "Password should be minimum eight characters, at least one uppercase letter and one number!",
  },
  validation: {
    emailRegex: "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",
    passwordRegex: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$",
    nameRegex: "^\\S{5,}$"
  }
};
